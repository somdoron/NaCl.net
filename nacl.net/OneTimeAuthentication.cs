using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nacl
{
  public class OneTimeAuthentication
  {
    private Poly1305.Poly1305 m_poly1305;

    public const int OutpubSize = 16;
    public const int KeySize = 32;

    public OneTimeAuthentication()
    {
      m_poly1305 = Poly1305.Poly1305.Create();
    }

    public void Authenticate(byte[] output, int outputOffset, byte[] input, int inputOffset, int inputLength, byte[] key,
      int keyOffset)
    {
      Authenticate((ArraySegment<byte>)output + outputOffset, (ArraySegment<byte>)input + inputOffset, inputLength, (ArraySegment<byte>)key + keyOffset);
    }

    public void Authenticate(byte[] output, byte[] input, byte[] key)
    {
      Authenticate((ArraySegment<byte>)output, (ArraySegment<byte>)input, input.Length, (ArraySegment<byte>)key);
    }

    internal void Authenticate(ArraySegment<byte> output, ArraySegment<byte> input, int inputLength, ArraySegment<byte> key)
    {
      m_poly1305.Key = key;

      m_poly1305.Transform(input, inputLength);
      m_poly1305.Finish(output);
    }

    public bool Verify(byte[] hash, byte[] input, byte[] key)
    {
      return Verify((ArraySegment<byte>) hash, (ArraySegment<byte>) input, input.Length, (ArraySegment<byte>) key);
    }

    public bool Verify(byte[] hash, int hashOffset, byte[] input,int inputOffset, int inputLength, byte[] key, int keyOffset)
    {
      return Verify((ArraySegment<byte>)hash + hashOffset, (ArraySegment<byte>)input + inputOffset, 
        inputLength, (ArraySegment<byte>)key + keyOffset);
    }

    internal bool Verify(ArraySegment<byte> hash, ArraySegment<byte> input, int inputLength, ArraySegment<byte> key)
    {
      ArraySegment<byte> correct = new ArraySegment<byte>(16);

      m_poly1305.Key = key;
      m_poly1305.Transform(input, inputLength);
      m_poly1305.Finish(correct);

      return SafeComparison.Verify16(hash, correct);
    }
  }
}
