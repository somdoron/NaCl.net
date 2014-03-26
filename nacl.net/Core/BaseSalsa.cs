using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace nacl.Core
{
  abstract class BaseSalsa20
  {
    public abstract int OutputSize { get; }
    public abstract int InputSize { get; }
    public abstract int KeySize { get; }
    public abstract int ConstSize { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Transform(byte[] output, byte[] input, byte[] k, byte[] c)
    {
      Transform(output, 0, input, 0, k, c);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void Transform(byte[] output, int outputOffset, byte[] input, int inputOffset, byte[] k, byte[] c);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected uint Rotate(uint u, int c)
    {
      unchecked
      {
        return (u << c) | (u >> (32 - c));
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected uint LoadLittleEndian(byte[] x, int offset)
    {
      unchecked
      {
        return (uint) (x[offset])
               | (((uint) (x[1 + offset])) << 8)
               | (((uint) (x[2 + offset])) << 16)
               | (((uint) (x[3 + offset])) << 24)
          ;
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void StoreLittleEndian(byte[] x,int offset, uint u)
    {
      unchecked
      {
        x[offset] = (byte) (u & 0xFF);
        u >>= 8;
        x[1 + offset] = (byte) (u & 0xFF);
        u >>= 8;
        x[2 + offset] = (byte) (u & 0xFF);
        u >>= 8;
        x[3 + offset] = (byte) (u & 0xFF);
      }
    }
  }
}
