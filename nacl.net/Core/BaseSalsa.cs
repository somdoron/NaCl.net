using System;
using System.Collections.Generic;
using System.Linq;
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

        public abstract void Transform(ArraySegment<byte> output, ArraySegment<byte> input, 
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
