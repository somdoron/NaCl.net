using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using nacl.Stream;

namespace nacl
{
  public class SecretBox
  {
    private XSalsa20Stream m_xSalsa20Stream;
    OneTimeAuthentication m_oneTimeAuthentication = new OneTimeAuthentication();

    /// <summary>
    /// The size of the secret box key in bytes
    /// </summary>
    public const int KeySize = 32;

    /// <summary>
    /// The size of the secret box nonce in bytes
    /// </summary>
    public const int NonceSize = 24;

    /// <summary>
    /// The amount of zeros in the begining of the plain message
    /// </summary>
    public const int ZeroSize = 32;

    /// <summary>
    /// The amount of zeros in the begining of a box message
    /// </summary>
    public const int BoxZeroSize = 16;

    /// <summary>
    /// The size of the MAC in a box message
    /// </summary>
    public const int MACSize = ZeroSize - BoxZeroSize;

    public SecretBox()
    {
      m_xSalsa20Stream = new XSalsa20Stream();
    }

    public SecretBox(byte[] key)
      : this()
    {
      Key = key;
    }

    public byte[] Key { get; set; }

    public void EasyBox(byte[] cipher, byte[] message, byte[] nonce)
    {
      byte[] cipherBoxed;
      byte[] messageBoxed;

      messageBoxed = new byte[message.Length + ZeroSize];
      Buffer.BlockCopy(message, 0, messageBoxed, ZeroSize, message.Length);

      cipherBoxed = new byte[message.Length + ZeroSize];

      Box(cipherBoxed, messageBoxed, nonce);
      Array.Clear(messageBoxed, 0, message.Length);

      Buffer.BlockCopy(cipherBoxed, BoxZeroSize, cipher,0 , message.Length + MACSize);
    }

    public void EaseOpen(byte[] message, byte[] cipher, byte[] nonce)
    {
      byte[] cipherBoxed;
      byte[] messageBoxed;

      cipherBoxed = new byte[cipher.Length + BoxZeroSize];
      Buffer.BlockCopy(cipher, 0, cipherBoxed, BoxZeroSize, cipher.Length);

      messageBoxed = new byte[cipher.Length + MACSize];

      Open(messageBoxed, cipherBoxed, nonce);

      Buffer.BlockCopy(messageBoxed, ZeroSize, message, 0, cipher.Length - MACSize);
    }

    public void Box(byte[] cipher, byte[] message, byte[] nonce)
    {
      Box(cipher,0, message,0, message.Length, nonce);
    }

    public void Box(byte[] cipher, int cipherOffset, byte[] message, int messageOffset, int messageLength, byte[] nonce)
    {     
      if (messageLength < ZeroSize || message.Length < messageLength)
      {
        throw new ArgumentException("messageLength must be greater than " + ZeroSize, "messageLength");
      }

      m_xSalsa20Stream.TransformXor(cipher, cipherOffset, message, messageOffset, messageLength, nonce,0, Key);
      
      m_oneTimeAuthentication.Authenticate(cipher,16 + cipherOffset, cipher, cipherOffset + 32, messageLength - 32, cipher, cipherOffset);

      Array.Clear(cipher,cipherOffset, BoxZeroSize);      
    }

    public void Open(byte[] message, byte[] cipher, byte[] nonce)
    {
      Open( message, 0, cipher,0, cipher.Length, nonce);
    }

    public void Open(byte[] message, int messageOffset, byte[] cipher, int cipherOffset, int cipherLength, byte[] nonce)
    {   
      byte[] subkey = new byte[KeySize];

      if (cipherLength < BoxZeroSize + MACSize)
      {
        throw new ArgumentException("cipherLength must be greater than 32", "cipherLength");
      }

      m_xSalsa20Stream.Transform(subkey, KeySize, nonce, Key);

      if (!m_oneTimeAuthentication.Verify(cipher , cipherOffset + BoxZeroSize, cipher, cipherOffset  + BoxZeroSize + MACSize,
        cipherLength - (BoxZeroSize + MACSize), subkey,0))
      {
        throw new SecurityException("mac verify failed");
      }

      m_xSalsa20Stream.TransformXor(message,messageOffset, cipher,cipherOffset, cipherLength, nonce,0, Key);

      Array.Clear(message, messageOffset, ZeroSize);      
    }
  }
}
