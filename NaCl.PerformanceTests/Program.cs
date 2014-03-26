using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;
using nacl;

namespace NaCl.PerformanceTests
{
  class Program
  {
    const int MsgLength = 250;  

    static void Main(string[] args)
    {
      Console.WriteLine("Primitive           | Avg μs\t| # Per Second");
      Console.WriteLine("----------------------------------------------");

      //ScalarMultiplication64Test();

      //ScalarMultiplication32Test();

      SecretBoxPerformanceTest();

      //AESHMACTest();

      Console.ReadLine();     
    }

    static void PrintPerformance(string primitive, long totalMilliseconds, int iterations)
    {
      if (primitive.Length > 20)
      {
        Console.Write("{0}|", primitive.Substring(0, 20));
      }
      else
      {
        Console.Write("{0}|", primitive.PadRight(20));
      }

      Console.WriteLine(" {0:N0}\t| {1:N0}", (totalMilliseconds * 1000.0) / iterations,
        (iterations / (double)totalMilliseconds) * 1000.0);
    }

    static private void AESHMACTest()
    {      
      const int msgNum = 30000;
      
      RandomNumberGenerator randomNumberGenerator = new RNGCryptoServiceProvider();
      
      byte[][] messages = new byte[msgNum][];
      byte[][] encryptMessages = new byte[msgNum][];

      AesManaged aesManaged = new AesManaged();
      HMACSHA256 hmacsha256 = new HMACSHA256();

      var encrypter = aesManaged.CreateEncryptor();

      int encryptMessageLength = (MsgLength / encrypter.InputBlockSize) * encrypter.InputBlockSize + encrypter.InputBlockSize + 
        hmacsha256.HashSize / 8;
           
      for (int i = 0; i < msgNum; i++)
      {
        messages[i] = new byte[MsgLength];
        encryptMessages[i] = new byte[encryptMessageLength];
        randomNumberGenerator.GetBytes(messages[i]);        
      }          

      // load caches
      for (int i = 0; i < 1000; i++)
      {        
        AESEncryptMessage(encrypter, hmacsha256, encryptMessages[i], messages[i]);
        Array.Clear(encryptMessages[i], 0, encryptMessageLength);
      }

      Stopwatch stopwatch = Stopwatch.StartNew();

      for (int i = 0; i < msgNum; i++)
      {
        AESEncryptMessage(encrypter, hmacsha256, encryptMessages[i], messages[i]);
      }

      stopwatch.Stop();

      PrintPerformance("AESSHA256 Encrypting", stopwatch.ElapsedMilliseconds, msgNum);

      byte[] message = new byte[MsgLength];
      var decrypter = aesManaged.CreateDecryptor();

      // load caches
      for (int i = 0; i < 1000; i++)
      {
        AESDecrypting(decrypter, hmacsha256, encryptMessages[i]);
      }

      stopwatch.Restart();

      for (int i = 0; i < msgNum; i++)
      {
        AESDecrypting(decrypter, hmacsha256, encryptMessages[i]);
      }

      stopwatch.Stop();

      PrintPerformance("AESSHA256 Decrypting", stopwatch.ElapsedMilliseconds, msgNum);
    }

    private static void AESDecrypting(ICryptoTransform decrypter, HMACSHA256 hmacsha256, byte[] encryptMessage)
    {
      byte[] finalBlock = decrypter.TransformFinalBlock(encryptMessage,0, encryptMessage.Length - hmacsha256.HashSize / 8);      

      byte[] hash = hmacsha256.ComputeHash(finalBlock);
      SafeComparison.Verify32(hash, 0, encryptMessage, encryptMessage.Length - hmacsha256.HashSize/8);
    }

    private static void AESEncryptMessage(ICryptoTransform encrypter, HMACSHA256 hmacsha256, 
      byte[] encryptMessage, byte[] message)
    {      
      byte[] finalBlock = encrypter.TransformFinalBlock(message, 0, message.Length);

      Buffer.BlockCopy(finalBlock, 0, encryptMessage, 0, finalBlock.Length);      

      byte[] hash = hmacsha256.ComputeHash(message);
      Buffer.BlockCopy(hash, 0, encryptMessage, finalBlock.Length, hash.Length);      
    }

    static private void SecretBoxPerformanceTest()
    {
      const int msgNum = 30000;          

      byte[] key = new byte[32];
      byte[] nonce = new byte[32];

      RandomNumberGenerator randomNumberGenerator =new RNGCryptoServiceProvider();
      randomNumberGenerator.GetBytes(key);
      randomNumberGenerator.GetBytes(nonce);

      byte[][] messages = new byte[msgNum][];
      byte[][] encryptMessages = new byte[msgNum][];

      for (int i = 0; i < msgNum; i++)
      {
        messages[i] = new byte[MsgLength + SecretBox.ZeroSize];
        encryptMessages[i] = new byte[MsgLength + SecretBox.ZeroSize];
        randomNumberGenerator.GetBytes(messages[i]);
        Array.Clear(messages[i], 0, SecretBox.ZeroSize);
      }

      SecretBox secretBox = new SecretBox(key);

      // load caches
      for (int i = 0; i < 1000; i++)
      {
        secretBox.Box(encryptMessages[i], messages[i], nonce);
        Array.Clear(encryptMessages[i], 0, MsgLength + SecretBox.ZeroSize);
      }      
      
      Stopwatch stopwatch = Stopwatch.StartNew();      

      for (int i = 0; i < msgNum; i++)
      {
        secretBox.Box(encryptMessages[i], messages[i], nonce);
      }

      stopwatch.Stop();

      PrintPerformance("XSalsaPoly1305 Enc", stopwatch.ElapsedMilliseconds, msgNum);

      byte[] message = new byte[MsgLength + SecretBox.ZeroSize];

      // load caches
      for (int i = 0; i < 1000; i++)
      {
        secretBox.Open(message, encryptMessages[i], nonce);
      }

      stopwatch.Restart();

      for (int i = 0; i < msgNum; i++)
      {
        secretBox.Open(message, encryptMessages[i], nonce);
      }

      stopwatch.Stop();

      PrintPerformance("XSalsaPoly1305 Dec", stopwatch.ElapsedMilliseconds, msgNum);
    }

    static private void ScalarMultiplication32Test()
    {
      byte[] key = new byte[32];

      for (int i = 0; i < 32; i++)
      {
        key[i] = 42;
      }

      byte[] result = new byte[32];

      const int iterations = 100;

      // load the caches
      for (int i = 0; i < 100; i++)
      {
        ScalarMultiplication32.MultiplyBase(result, key);
      }

      Stopwatch stopwatch = Stopwatch.StartNew();

      for (int i = 0; i < iterations; i++)
      {
        ScalarMultiplication32.MultiplyBase(result, key);
      }

      stopwatch.Stop();

      PrintPerformance("ScalarMult32", stopwatch.ElapsedMilliseconds, iterations);      
    }

    static private void ScalarMultiplication64Test()
    {
      byte[] key = new byte[32];      

      for (int i = 0; i < 32; i++)
      {
        key[i] = 42;
      }

      byte[] result = new byte[32];

      const int iterations = 100;

      // load the caches
      for (int i = 0; i < 100; i++)
      {
        ScalarMultiplication64.MultiplyBase(result, key);
      }

      Stopwatch stopwatch = Stopwatch.StartNew();

      for (int i = 0; i < iterations; i++)
      {
        ScalarMultiplication64.MultiplyBase(result, key);  
      }
      
      stopwatch.Stop();

      PrintPerformance("ScalarMult64", stopwatch.ElapsedMilliseconds, iterations);
    }
  }
}
