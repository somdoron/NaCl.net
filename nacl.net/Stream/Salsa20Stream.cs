using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nacl.Core;

namespace nacl.Stream
{
  class Salsa20Stream : BaseStream
  {
    Salsa20 m_salsa20 = new Salsa20();
    private byte[] m_block;
    private byte[] m_input;

    public Salsa20Stream()
    {
      m_block = new byte[64];
      m_input = new byte[16];
    }

    public override int KeySize
    {
      get { return 32; }
    }

    public override int NonceSize
    {
      get
      {
        return 8;
      }
    }

    public override void Transform(byte[] cipher, int cipherOffset, int cipherLength, byte[] nonce, int nonceOffset, byte[] key)
    {
      if (cipherLength == 0)
        return;

      for (int i = 0; i < 8; ++i)
      {
        m_input[i] = nonce[i + nonceOffset];
      }

      for (int i = 8; i < 16; ++i)
      {
        m_input[i] = 0;
      }

      while (cipherLength >= 64)
      {
        m_salsa20.Transform(cipher, cipherOffset, m_input,0, key, Constants.Sigma);

        uint u = 1;
        for (int i = 8; i < 16; ++i)
        {
          u += (uint)m_input[i];
          m_input[i] = (byte)(u & 0xFF);
          u >>= 8;
        }

        cipherLength -= 64;
        cipherOffset += 64;
      }

      if (cipherLength != 0)
      {
        m_salsa20.Transform(m_block, m_input, key, Constants.Sigma);

        for (int i = 0; i < cipherLength; ++i)
        {
          cipher[i + cipherOffset] = m_block[i];
        }
      }
    }

    public override void TransformXor(byte[] cipher, int cipherOffset, byte[] message, int messageOffset, int messageLength, byte[] nonce, int nonceOffset, byte[] key)
    {
      if (messageLength == 0) return;

      for (int i = 0; i < 8; ++i)
      {
        m_input[i] = nonce[i + nonceOffset];
      }

      for (int i = 8; i < 16; ++i)
      {
        m_input[i] = 0;
      }

      while (messageLength >= 64)
      {
        m_salsa20.Transform(m_block, m_input, key, Constants.Sigma);
        for (int i = 0; i < 64; ++i)
        {
          cipher[i + cipherOffset] = (byte)(message[i + messageOffset] ^ m_block[i]);
        }

        uint u = 1;
        for (int i = 8; i < 16; ++i)
        {
          u += m_input[i];
          m_input[i] = (byte)(u & 0xff);
          u >>= 8;
        }

        messageLength -= 64;
        cipherOffset += 64;
        messageOffset += 64;
      }

      if (messageLength != 0)
      {
        m_salsa20.Transform(m_block, m_input, key, Constants.Sigma);

        for (int i = 0; i < messageLength; ++i)
        {
          cipher[i + cipherOffset] = (byte)(message[i + messageOffset] ^ m_block[i]);
        }          
      }
    }
  }
}
