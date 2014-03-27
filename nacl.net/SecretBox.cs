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
  /// <summary>
  /// The secret box encrypts, decrypts, authenticates and verify a message using a secret key and nonce.
  /// </summary>
  /// <remarks>
  /// The secret box is design to meet the standard notion of privacy and authenticity for secret-key authenticated-encryption scheme using nonces.
  /// Note that the length is not hidden. 
  /// Note also that it is the caller's responsibility to ensure the uniqueness of nonces—for example, by using nonce 1 for the first message, nonce 2 for the second message, etc. 
  /// Nonces are long enough that randomly generated nonces have negligible risk of collision.
  /// Secret box is a combination of Salsa20 and Poly1305.  
  /// Secret box is not thread safe.
  /// </remarks>
  public class SecretBox
  {
    private XSalsa20Stream m_xSalsa20Stream;
    OneTimeAuthentication m_oneTimeAuthentication = new OneTimeAuthentication();
    private byte[] m_subkey = new byte[KeySize];

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

    /// <summary>
    /// Create a secret box and initialize the key
    /// </summary>
    /// <param name="key">Key for the secret box</param>
    public SecretBox(byte[] key)
      : this()
    {
      Key = key;
    }

    /// <summary>
    /// The secret key of the secret box, used to encrypt and authenticate the messages
    /// </summary>
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

    /// <summary>
    /// The method encrypts and authenticates a message <paramref name="message"/> using a secret key <see cref="Key"/> and a nonce <paramref name="nonce"/>. 
    /// The method puts the ciphertext into <paramref name="cipher"/>
    /// </summary>
    /// <param name="cipher">Output for the ciphertext</param>
    /// <param name="message">The message to encrypt and authenticate</param>
    /// <param name="nonce">The nonce use to encrypt and authenticate the message</param>
    public void Box(byte[] cipher, byte[] message, byte[] nonce)
    {
      Box(cipher,0, message,0, message.Length, nonce);
    }
    /// <summary>
    /// The method encrypts and authenticates a message <paramref name="message"/> using a secret key <see cref="Key"/> and a nonce <paramref name="nonce"/>. 
    /// The method puts the ciphertext into <paramref name="cipher"/>
    /// </summary>
    /// <param name="cipher">Output for the ciphertext</param>
    /// <param name="cipherOffset">The index in <paramref name="cipher"/> at which storing begins</param>
    /// <param name="message">The message to encrypt and authenticate, first 32 bytes of the byte array must be zeros.</param>
    /// <param name="messageOffset">The index in <paramref name="message"/> from which to begin using data.</param>
    /// <param name="messageLength">The number of bytes in the <paramref name="message"/> to use as data. Must be greater than 32.</param>
    /// <param name="nonce">The nonce use to encrypt and authenticate the message.</param>
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

    /// <summary>
    /// The method verifies and decrypts a ciphertext <paramref name="cipher"/> using secret key <see cref="Key"/> and a nonce <paramref name="nonce"/> 
    /// The method puts the plaintext into <paramref name="message"/>.
    /// </summary>
    /// <param name="message">Output for the plaintext</param>
    /// <param name="cipher">The ciphertext</param>
    /// <param name="nonce">The nonce that used to encrypt the ciphertext and authenticate the message</param>
    public void Open(byte[] message, byte[] cipher, byte[] nonce)
    {
      Open( message, 0, cipher,0, cipher.Length, nonce);
    }

    /// <summary>
    /// The method verifies and decrypts a ciphertext <paramref name="cipher"/> using secret key <see cref="Key"/> and a nonce <paramref name="nonce"/> 
    /// The method puts the plaintext into <paramref name="message"/>.
    /// </summary>
    /// <param name="message">Output for the plaintext</param>
    /// <param name="messageOffset">The index in <paramref name="message"/> at which storing begins</param>
    /// <param name="cipher">The ciphertext</param>
    /// <param name="cipherOffset">The index in <paramref name="cipher"/> from which to begin using data.</param>
    /// <param name="cipherLength">The number of bytes in the <paramref name="cipher"/> to use as data.</param>
    /// <param name="nonce">The nonce that used to encrypt the ciphertext and authenticate the message</param>
    public void Open(byte[] message, int messageOffset, byte[] cipher, int cipherOffset, int cipherLength, byte[] nonce)
    {
      if (cipherLength < BoxZeroSize + MACSize)
      {
        throw new ArgumentException("cipherLength must be greater than 32", "cipherLength");
      }

      m_xSalsa20Stream.Transform(m_subkey, KeySize, nonce, Key);

      if (!m_oneTimeAuthentication.Verify(cipher , cipherOffset + BoxZeroSize, cipher, cipherOffset  + BoxZeroSize + MACSize,
        cipherLength - (BoxZeroSize + MACSize), m_subkey,0))
      {
        throw new SecurityException("mac verify failed");
      }

      m_xSalsa20Stream.TransformXor(message,messageOffset, cipher,cipherOffset, cipherLength, nonce,0, Key);

      Array.Clear(message, messageOffset, ZeroSize);
      Array.Clear(m_subkey, 0, m_subkey.Length);
    }
  }
}
