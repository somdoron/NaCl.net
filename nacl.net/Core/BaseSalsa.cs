using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nacl.Core
{
    public abstract class BaseSalsa20  : ISalsa20
    {
        public abstract int OutputBytes { get; }
        public abstract int InputBytes { get; }
        public abstract int KeyBytes { get; }
        public abstract int ConstBytes { get; }
        
        public void Transform(byte[] @out, byte[] @in, byte[] k, byte[] c)
        {
            TransformInternal(@out, @in, k, c);
        }

        protected abstract void TransformInternal(ArraySegment<byte> @out, ArraySegment<byte> @in, 
            ArraySegment<byte> k, ArraySegment<byte> c);

        protected uint Rotate(uint u, int c)
        {
            return (u << c) | (u >> (32 - c));
        }

        protected uint LoadLittleEndian(ArraySegment<byte> x)
        {
            return (uint)(x[0])
            | (((uint)(x[1])) << 8)
            | (((uint)(x[2])) << 16)
            | (((uint)(x[3])) << 24)
            ;
        }

        protected void StoreLittleEndian(ArraySegment<byte> x, uint u)
        {
            x[0] = (byte)(u & 0xFF);
            u >>= 8;
            x[1] = (byte)(u & 0xFF);
            u >>= 8;
            x[2] = (byte)(u & 0xFF);
            u >>= 8;
            x[3] = (byte)(u & 0xFF);
        }
    }
}
