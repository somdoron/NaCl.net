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

    public void Box(byte[] cipher, byte[] message, byte[] nonce)
    {
      Box((ArraySegment<byte>)cipher, (ArraySegment<byte>)message, message.Length, (ArraySegment<byte>)nonce);
    }

    public void Box(byte[] cipher, int cipherOffset, byte[] message, int messageOffset, int messageLength, byte[] nonce,
      int nonceOffset)
    {
      Box((ArraySegment<byte>)cipher + cipherOffset, (ArraySegment<byte>)message + messageOffset, 
        messageLength, (ArraySegment<byte>)nonce + nonceOffset);
    }

    internal void Box(ArraySegment<byte> cipher, ArraySegment<byte> message, int messageLength, ArraySegment<byte> nonce)
    {
      if (messageLength < ZeroSize || message.Count < messageLength)
      {
        throw new ArgumentException("messageLength must be greater than " + ZeroSize, "messageLength");
      }

      m_xSalsa20Stream.TransformXor(cipher, message, messageLength, nonce, Key);

      m_oneTimeAuthentication.Authenticate(cipher + 16, cipher + 32, messageLength - 32, cipher);

      for (int i = 0; i < BoxZeroSize; ++i)
        cipher[i] = 0;
    }

    public void Open(byte[] message, byte[] cipher, byte[] nonce)
    {
      Open((ArraySegment<byte>) message, (ArraySegment<byte>) cipher, cipher.Length, (ArraySegment<byte>) nonce);
    }

    public void Open(byte[] message, int messageOffset, byte[] cipher, int cipherOffset, int cipherLength, byte[] nonce, int nonceOffset)
    {
      Open((ArraySegment<byte>)message + messageOffset, (ArraySegment<byte>)cipher + cipherOffset, cipherLength, (ArraySegment<byte>)nonce + nonceOffset);
    }

    internal void Open(ArraySegment<byte> message, ArraySegment<byte> cipher, int cipherLength, ArraySegment<byte> nonce)
    {
      byte[] subkey = new byte[KeySize];

      if (cipherLength < BoxZeroSize + MACSize)
      {
        throw new ArgumentException("cipherLength must be greater than 32", "cipherLength");
      }

      m_xSalsa20Stream.Transform(subkey, KeySize, nonce, Key);

      if (!m_oneTimeAuthentication.Verify((ArraySegment<byte>)cipher + BoxZeroSize, (ArraySegment<byte>)cipher + BoxZeroSize + MACSize,
        cipherLength - (BoxZeroSize + MACSize), subkey))
      {
        throw new SecurityException("mac verify failed");
      }

      m_xSalsa20Stream.TransformXor(message, cipher, cipherLength, nonce, Key);

      for (int i = 0; i < ZeroSize; ++i)
        message[i] = 0;
    }
  }
}
