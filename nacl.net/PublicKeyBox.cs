using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using nacl.Core;

namespace nacl
{
  public class PublicKeyBox
  {
    public const int PublicKeyBytes = 32;
    
    public const int SecretKeyBytes = 32;
    
    public const int NonceBytes = 24;
    
    public const int ZeroBytes = 32;
    
    public const int BoxZeroBytes = 16;
    
    public const int MACBytes = ZeroBytes - BoxZeroBytes;
        
    private readonly SecretBox m_secretBox;

    public KeyPair GenerateKeyPair()
    {     
      byte[] publickKey = new byte[PublicKeyBytes];
      byte[] secretKey = new byte[SecretKeyBytes];

      RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();

      random.GetBytes(secretKey);

      ScalarMultiplication.MultiplyBase(publickKey, secretKey);

      return new KeyPair(publickKey, secretKey);
    }

    public PublicKeyBox(byte[] publicKey, byte[] secretKey)
    {
      HSalsa20 salsa20 = new HSalsa20();

      byte[] s = new byte[salsa20.KeyBytes];
      ScalarMultiplication.Multiply(s, secretKey, publicKey);

      byte[] key = new byte[salsa20.OutputBytes];
      
      salsa20.Transform(key, Constants.N, s, Constants.Sigma);

      m_secretBox = new SecretBox(key);
    }

    public void Box(byte[] cipher, byte[] message, int messageLength, byte[] nonce)
    {
      m_secretBox.Box(cipher, message, messageLength, nonce);
    }

    public void Open(byte[] message, byte[] cipher, int cipherLength, byte[] nonce)
    {
      m_secretBox.Open(message, cipher, cipherLength, nonce);
    }
  }
}
