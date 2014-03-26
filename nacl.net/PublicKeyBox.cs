using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using nacl.Core;

namespace nacl
{
  public class PublicKeyBox
  {
    public const int PublicKeySize = 32;
    
    public const int SecretKeySize = 32;
    
    public const int NonceSize = 24;
    
    public const int ZeroSize = 32;
    
    public const int BoxZeroSize = 16;
    
    public const int MACSize = ZeroSize - BoxZeroSize;
        
    private readonly SecretBox m_secretBox;
   
    HSalsa20 m_salsa20 = new HSalsa20();

    public PublicKeyBox(byte[] publicKey, byte[] secretKey)
    {
      m_secretBox = new SecretBox();
      SetKeys(publicKey, secretKey);
    }

    public void SetKeys(byte[] publicKey, byte[] secretKey)
    {
      byte[] s = new byte[ScalarMultiplication.OutputSize];
      ScalarMultiplication.Multiply(s, secretKey, publicKey);

      byte[] key = new byte[m_salsa20.OutputSize];

      m_salsa20.Transform(key, Constants.N, s, Constants.Sigma);
      m_secretBox.Key = key;
    }

    public KeyPair GenerateKeyPair()
    {
      byte[] publickKey = new byte[PublicKeySize];
      byte[] secretKey = new byte[SecretKeySize];

      RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();

      random.GetBytes(secretKey);

      ScalarMultiplication.MultiplyBase(publickKey, secretKey);

      return new KeyPair(publickKey, secretKey);
    }

    public void Box(byte[] cipher, byte[] message, byte[] nonce)
    {
      m_secretBox.Box(cipher, message, nonce);
    }

    public void Box(byte[] cipher, int cipherOffset, byte[] message, int messageOffset,int messageLength, byte[] nonce)
    {
      m_secretBox.Box(cipher, cipherOffset, message, messageOffset, messageLength, nonce);
    }

    public void Open(byte[] message, byte[] cipher, byte[] nonce)
    {
      m_secretBox.Open(message, cipher, nonce);
    }

    public void Open(byte[] message, int messageOffset, byte[] cipher, int cipherOffset, int cipherLength, byte[] nonce)
    {
      m_secretBox.Open(message, messageOffset, cipher, cipherOffset, cipherLength, nonce);
    }
  }
}
