using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using nacl.Stream;

namespace nacl
{
  public class SecretBox
  {
    private byte[] m_key;
    private XSalsa20Stream m_xSalsa20Stream;

    public const int KeyBytes = 32;    
    public const int NonceBytes = 24;
    public const int ZeroBytes = 32;
    public const int BoxZeroBytes = 16;
    public const int MACBytes = ZeroBytes - BoxZeroBytes;

    public SecretBox(byte[] key)
    {
      m_key = key;
      m_xSalsa20Stream = new XSalsa20Stream();
    }

    public void Box(byte[] cipher, byte[] message, int messageLength, byte[] nonce)
    {
      if (messageLength < 32)
      {
        throw new ArgumentException("messageLength must be greater than 32", "messageLength");
      }

      m_xSalsa20Stream.TransformXor(cipher, message, messageLength, nonce, m_key);

      Poly1305.Poly1305.Auth((ArraySegment<byte>)cipher + 16, (ArraySegment<byte>)cipher + 32, messageLength-32, cipher);      

      for (int i = 0; i < 16; ++i)
        cipher[i] = 0;
    }

    public void Open(byte[] message, byte[] cipher, int cipherLength, byte[] nonce)
    {
      byte[] subkey = new byte[32];

      if (cipherLength < 32)
      {
        throw new ArgumentException("cipherLength must be greater than 32", "cipherLength");
      }

      m_xSalsa20Stream.Transform(subkey, 32, nonce, m_key);

      byte[] mac = new byte[16];

      Poly1305.Poly1305.Auth(mac, (ArraySegment<byte>)cipher + 32, cipherLength - 32, subkey);

      if (!Poly1305.Poly1305.Verify((ArraySegment<byte>) cipher + 16, mac))
      {
        throw new SecurityException("mac verify failed");
      }

      m_xSalsa20Stream.TransformXor(message, cipher, cipherLength, nonce, m_key);

      for (int i = 0; i < 32; ++i) 
        message[i] = 0;
    }
  }
}
