using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nacl.Core
{
    class Salsa20 : BaseSalsa20
    {
        private const int Rounds = 20;        

        public override int OutputBytes
        {
            get { return 64; }
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

        public override void Transform(ArraySegment<byte> output, ArraySegment<byte> input,
            ArraySegment<byte> k, ArraySegment<byte> c)
        {
            uint x0, x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15;
            uint j0, j1, j2, j3, j4, j5, j6, j7, j8, j9, j10, j11, j12, j13, j14, j15;
            int i;

            j0 = x0 = LoadLittleEndian(c + 0);
            j1 = x1 = LoadLittleEndian(k + 0);
            j2 = x2 = LoadLittleEndian(k + 4);
            j3 = x3 = LoadLittleEndian(k + 8);
            j4 = x4 = LoadLittleEndian(k + 12);
            j5 = x5 = LoadLittleEndian(c + 4);
            j6 = x6 = LoadLittleEndian(input + 0);
            j7 = x7 = LoadLittleEndian(input + 4);
            j8 = x8 = LoadLittleEndian(input + 8);
            j9 = x9 = LoadLittleEndian(input + 12);
            j10 = x10 = LoadLittleEndian(c + 8);
            j11 = x11 = LoadLittleEndian(k + 16);
            j12 = x12 = LoadLittleEndian(k + 20);
            j13 = x13 = LoadLittleEndian(k + 24);
            j14 = x14 = LoadLittleEndian(k + 28);
            j15 = x15 = LoadLittleEndian(c + 12);

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

            x0 += j0;
            x1 += j1;
            x2 += j2;
            x3 += j3;
            x4 += j4;
            x5 += j5;
            x6 += j6;
            x7 += j7;
            x8 += j8;
            x9 += j9;
            x10 += j10;
            x11 += j11;
            x12 += j12;
            x13 += j13;
            x14 += j14;
            x15 += j15;

            StoreLittleEndian(output + 0, x0);
            StoreLittleEndian(output + 4, x1);
            StoreLittleEndian(output + 8, x2);
            StoreLittleEndian(output + 12, x3);
            StoreLittleEndian(output + 16, x4);
            StoreLittleEndian(output + 20, x5);
            StoreLittleEndian(output + 24, x6);
            StoreLittleEndian(output + 28, x7);
            StoreLittleEndian(output + 32, x8);
            StoreLittleEndian(output + 36, x9);
            StoreLittleEndian(output + 40, x10);
            StoreLittleEndian(output + 44, x11);
            StoreLittleEndian(output + 48, x12);
            StoreLittleEndian(output + 52, x13);
            StoreLittleEndian(output + 56, x14);
            StoreLittleEndian(output + 60, x15);
        }

        

    }
}
