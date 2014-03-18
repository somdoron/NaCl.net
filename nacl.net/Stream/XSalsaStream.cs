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

    public override int KeyBytes
    {
      get { return 32; }
    }

    public override int NonceBytes
    {
      get { return 24; }
    }

    public override void Transform(ArraySegment<byte> cipher, int cipherLength, ArraySegment<byte> nonce, ArraySegment<byte> key)
    {
      byte[] subkey = new byte[32];

      m_hSalsa20.Transform(subkey, nonce, key, Constants.Sigma);

      base.Transform(cipher, cipherLength, nonce + 16, subkey);
    }

    public override void TransformXor(ArraySegment<byte> cipher, ArraySegment<byte> message, int messageLength, ArraySegment<byte> nonce, ArraySegment<byte> key)
    {
      byte[] subkey = new byte[32];

      m_hSalsa20.Transform(subkey, nonce, key, Constants.Sigma);

      base.TransformXor(cipher, message, messageLength, nonce + 16, subkey);
    }
  }
}
