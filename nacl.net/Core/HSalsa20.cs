using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nacl.Core
{
    public class HSalsa20 : BaseSalsa20
    {
        private const int Rounds = 20;

        public override int OutputBytes
        {
            get { return 32; }
        }

        public override int InputBytes
        {
            get { return 16; }
        }

        public override int KeyBytes
        {
            get { return 32; }
        }

        public override int ConstBytes
        {
            get { return 16; }
        }

        protected override void TransformInternal(ArraySegment<byte> @out, ArraySegment<byte> @in, ArraySegment<byte> k, ArraySegment<byte> c)
        {
            uint x0, x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15;
            int i;

            x0 = LoadLittleEndian(c + 0);
            x1 = LoadLittleEndian(k + 0);
            x2 = LoadLittleEndian(k + 4);
            x3 = LoadLittleEndian(k + 8);
            x4 = LoadLittleEndian(k + 12);
            x5 = LoadLittleEndian(c + 4);
            x6 = LoadLittleEndian(@in + 0);
            x7 = LoadLittleEndian(@in + 4);
            x8 = LoadLittleEndian(@in + 8);
            x9 = LoadLittleEndian(@in + 12);
            x10 = LoadLittleEndian(c + 8);
            x11 = LoadLittleEndian(k + 16);
            x12 = LoadLittleEndian(k + 20);
            x13 = LoadLittleEndian(k + 24);
            x14 = LoadLittleEndian(k + 28);
            x15 = LoadLittleEndian(c + 12);

            for (i = Rounds; i > 0; i -= 2)
            {
                x4 ^= Rotate(x0 + x12, 7);
                x8 ^= Rotate(x4 + x0, 9);
                x12 ^= Rotate(x8 + x4, 13);
                x0 ^= Rotate(x12 + x8, 18);
                x9 ^= Rotate(x5 + x1, 7);
                x13 ^= Rotate(x9 + x5, 9);
                x1 ^= Rotate(x13 + x9, 13);
                x5 ^= Rotate(x1 + x13, 18);
                x14 ^= Rotate(x10 + x6, 7);
                x2 ^= Rotate(x14 + x10, 9);
                x6 ^= Rotate(x2 + x14, 13);
                x10 ^= Rotate(x6 + x2, 18);
                x3 ^= Rotate(x15 + x11, 7);
                x7 ^= Rotate(x3 + x15, 9);
                x11 ^= Rotate(x7 + x3, 13);
                x15 ^= Rotate(x11 + x7, 18);
                x1 ^= Rotate(x0 + x3, 7);
                x2 ^= Rotate(x1 + x0, 9);
                x3 ^= Rotate(x2 + x1, 13);
                x0 ^= Rotate(x3 + x2, 18);
                x6 ^= Rotate(x5 + x4, 7);
                x7 ^= Rotate(x6 + x5, 9);
                x4 ^= Rotate(x7 + x6, 13);
                x5 ^= Rotate(x4 + x7, 18);
                x11 ^= Rotate(x10 + x9, 7);
                x8 ^= Rotate(x11 + x10, 9);
                x9 ^= Rotate(x8 + x11, 13);
                x10 ^= Rotate(x9 + x8, 18);
                x12 ^= Rotate(x15 + x14, 7);
                x13 ^= Rotate(x12 + x15, 9);
                x14 ^= Rotate(x13 + x12, 13);
                x15 ^= Rotate(x14 + x13, 18);
            }

            StoreLittleEndian(@out + 0, x0);
            StoreLittleEndian(@out + 4, x5);
            StoreLittleEndian(@out + 8, x10);
            StoreLittleEndian(@out + 12, x15);
            StoreLittleEndian(@out + 16, x6);
            StoreLittleEndian(@out + 20, x7);
            StoreLittleEndian(@out + 24, x8);
            StoreLittleEndian(@out + 28, x9);
        }
    }
}
