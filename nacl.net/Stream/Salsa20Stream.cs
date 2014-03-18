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

    public override int KeyBytes
    {
      get { return 32; }
    }

    public override int NonceBytes
    {
      get
      {
        return 8;
      }
    }

    public override void Transform(ArraySegment<byte> cipher, int cipherLength,
      ArraySegment<byte> nonce, ArraySegment<byte> key)
    {
      byte[] input = new byte[16];
      byte[] block = new byte[64];

      if (cipherLength == 0)
        return;

      for (int i = 0; i < 8; ++i)
      {
        input[i] = nonce[i];
      }

      for (int i = 8; i < 16; ++i)
      {
        input[i] = 0;
      }

      while (cipherLength >= 64)
      {
        m_salsa20.Transform(cipher, input, key, Constants.Sigma);

        uint u = 1;
        for (int i = 8; i < 16; ++i)
        {
          u += (uint)input[i];
          input[i] = (byte)(u & 0xFF);
          u >>= 8;
        }

        cipherLength -= 64;
        cipher += 64;
      }

      if (cipherLength != 0)
      {
        m_salsa20.Transform(block, input, key, Constants.Sigma);

        for (int i = 0; i < cipherLength; ++i)
        {
          cipher[i] = block[i];
        }
      }
    }

    public override void TransformXor(ArraySegment<byte> cipher, ArraySegment<byte> message, int messageLength, ArraySegment<byte> nonce, ArraySegment<byte> key)
    {
      byte[] input = new byte[16];
      byte[] block = new byte[64];

      if (messageLength == 0) return;

      for (int i = 0; i < 8; ++i)
      {
        input[i] = nonce[i];
      }

      for (int i = 8; i < 16; ++i)
      {
        input[i] = 0;
      }

      while (messageLength >= 64)
      {
        m_salsa20.Transform(block, input, key, Constants.Sigma);
        for (int i = 0; i < 64; ++i)
        {
          cipher[i] = (byte)(message[i] ^ block[i]);
        }

        uint u = 1;
        for (int i = 8; i < 16; ++i)
        {
          u += input[i];
          input[i] = (byte)(u & 0xff);
          u >>= 8;
        }

        messageLength -= 64;
        cipher += 64;
        message += 64;
      }

      if (messageLength != 0)
      {
        m_salsa20.Transform(block, input, key, Constants.Sigma);

        for (int i = 0; i < messageLength; ++i)
        {
          cipher[i] = (byte)(message[i] ^ block[i]);
        }          
      }
    }
  }
}
