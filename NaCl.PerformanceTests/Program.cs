using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using nacl;

namespace NaCl.PerformanceTests
{
  class Program
  {
    static void Main(string[] args)
    {
      //ScalarMultiplication64Test();

      //ScalarMultiplication32Test();

      SecretBoxPerformanceTest();

      Console.ReadLine();     
    }

    static private void SecretBoxPerformanceTest()
    {
      const int msgNum = 30000;
      const int msgLength = 250;      

      byte[] key = new byte[32];
      byte[] nonce = new byte[32];

      RandomNumberGenerator randomNumberGenerator =new RNGCryptoServiceProvider();
      randomNumberGenerator.GetBytes(key);
      randomNumberGenerator.GetBytes(nonce);

      byte[][] messages = new byte[msgNum][];
      byte[][] encryptMessages = new byte[msgNum][];

      for (int i = 0; i < msgNum; i++)
      {
        messages[i] = new byte[msgLength];   
        encryptMessages[i] =new byte[msgLength];
        randomNumberGenerator.GetBytes(messages[i]);
        Array.Clear(messages[i], 0, SecretBox.ZeroSize);
      }

      SecretBox secretBox = new SecretBox(key);

      // load caches
      for (int i = 0; i < 1000; i++)
      {
        secretBox.Box(encryptMessages[i], messages[i], nonce);
        Array.Clear(encryptMessages[i], 0, msgLength);
      }      
      
      Stopwatch stopwatch = Stopwatch.StartNew();      

      for (int i = 0; i < msgNum; i++)
      {
        secretBox.Box(encryptMessages[i], messages[i], nonce);
      }

      stopwatch.Stop();

      Console.WriteLine("Encrypting {1} byte message took {0:N0} microsecond per message",
        (stopwatch.ElapsedMilliseconds * 1000.0) / msgNum, msgLength);
      Console.WriteLine("Encrypting {0:N0} messages per second",  
        (msgNum / (double)stopwatch.ElapsedMilliseconds) * 1000.0);      

      byte[] message =new byte[msgLength];

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

      Console.WriteLine("Decrypting {1} byte message took {0:N0} microsecond per message",
        (stopwatch.ElapsedMilliseconds * 1000.0) / msgNum, msgLength);
      Console.WriteLine("Decrypting {0:N0} messages per second",
        (msgNum / (double)stopwatch.ElapsedMilliseconds) * 1000.0);
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

      Console.WriteLine("32bit avg {0} microseconds", stopwatch.ElapsedMilliseconds * 1000.0 / iterations);
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

      Console.WriteLine("64bit avg {0} microseconds", stopwatch.ElapsedMilliseconds * 1000.0 / iterations);
    }
  }
}
