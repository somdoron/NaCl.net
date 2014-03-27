using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nacl.Core;

namespace nacl.Stream
{
  class XSalsa20Stream : Salsa20Stream
  {
    HSalsa20 m_hSalsa20 = new HSalsa20();
    private byte[] m_subkey = new byte[32];

    public override int KeySize
    {
      get { return 32; }
    }

    public override int NonceSize
    {
      get { return 24; }
    }

    public override void Transform(byte[] cipher, int cipherOffset, int cipherLength, byte[] nonce, int nonceOffset, byte[] key)
    {
      m_hSalsa20.Transform(m_subkey,0, nonce,nonceOffset, key, Constants.Sigma);

      base.Transform(cipher, cipherOffset, cipherLength, nonce, 16 + nonceOffset, m_subkey);
    }

    public override void TransformXor(byte[] cipher, int cipherOffset, byte[] message, int messageOffset, int messageLength, byte[] nonce, int nonceOffset, byte[] key)
    {   
      m_hSalsa20.Transform(m_subkey,0, nonce,nonceOffset, key, Constants.Sigma);

      base.TransformXor(cipher, cipherOffset, message, messageOffset, messageLength, nonce, 16 + nonceOffset, m_subkey);
    }
  }
}
