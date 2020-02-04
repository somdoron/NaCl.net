using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("NaCl.Tests, PublicKey=002400000480000094000000060" +
                              "200000024000052534131000400000100010067b1aa21413f594425a88383ca0" +
                              "9bcc62c34092da0cde540f8929cc026e39cf5f862faa23d96616e7050b23fd43" +
                              "1a1007a0866c9e08aa8b1d5fdc5021f09b5302bb8545f4db589c60622f5a31d7" +
                              "18896af939388230dd3d407fffa623dd7e75f6fef38da5bcba0b1d109f7f5ad6" +
                              "4344bd24b995c744ca704640f511aa3ee3ba7")]
namespace NaCl.Internal
{
    internal class HSalsa20
    {
        private const int Rounds = 20;

        public const int BlockLength = 32;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Transform(Span<byte> output, ReadOnlySpan<byte> input, ReadOnlySpan<byte> k, ReadOnlySpan<byte> c)
        {
            UInt32 x0;
            UInt32 x1;
            UInt32 x2;
            UInt32 x3;
            UInt32 x4;
            UInt32 x5;
            UInt32 x6;
            UInt32 x7;
            UInt32 x8;
            UInt32 x9;
            UInt32 x10;
            UInt32 x11;
            UInt32 x12;
            UInt32 x13;
            UInt32 x14;
            UInt32 x15;

            if (c.IsEmpty)
            {
                x0 = 0x61707865U;
                x5 = 0x3320646eU;
                x10 = 0x79622d32U;
                x15 = 0x6b206574U;
            }
            else
            {
                x0 = Common.Load32(c, 0);
                x5 = Common.Load32(c, 4);
                x10 = Common.Load32(c, 8);
                x15 = Common.Load32(c, 12);
            }

            x1 = Common.Load32(k, 0);
            x2 = Common.Load32(k, 4);
            x3 = Common.Load32(k, 8);
            x4 = Common.Load32(k, 12);
            x11 = Common.Load32(k, 16);
            x12 = Common.Load32(k, 20);
            x13 = Common.Load32(k, 24);
            x14 = Common.Load32(k, 28);
            x6 = Common.Load32(input, 0);
            x7 = Common.Load32(input, 4);
            x8 = Common.Load32(input, 8);
            x9 = Common.Load32(input, 12);

            for (int i = Rounds; i > 0; i -= 2)
            {
                x4 ^= Common.RotateLeft(x0 + x12, 7);
                x8 ^= Common.RotateLeft(x4 + x0, 9);
                x12 ^= Common.RotateLeft(x8 + x4, 13);
                x0 ^= Common.RotateLeft(x12 + x8, 18);
                x9 ^= Common.RotateLeft(x5 + x1, 7);
                x13 ^= Common.RotateLeft(x9 + x5, 9);
                x1 ^= Common.RotateLeft(x13 + x9, 13);
                x5 ^= Common.RotateLeft(x1 + x13, 18);
                x14 ^= Common.RotateLeft(x10 + x6, 7);
                x2 ^= Common.RotateLeft(x14 + x10, 9);
                x6 ^= Common.RotateLeft(x2 + x14, 13);
                x10 ^= Common.RotateLeft(x6 + x2, 18);
                x3 ^= Common.RotateLeft(x15 + x11, 7);
                x7 ^= Common.RotateLeft(x3 + x15, 9);
                x11 ^= Common.RotateLeft(x7 + x3, 13);
                x15 ^= Common.RotateLeft(x11 + x7, 18);
                x1 ^= Common.RotateLeft(x0 + x3, 7);
                x2 ^= Common.RotateLeft(x1 + x0, 9);
                x3 ^= Common.RotateLeft(x2 + x1, 13);
                x0 ^= Common.RotateLeft(x3 + x2, 18);
                x6 ^= Common.RotateLeft(x5 + x4, 7);
                x7 ^= Common.RotateLeft(x6 + x5, 9);
                x4 ^= Common.RotateLeft(x7 + x6, 13);
                x5 ^= Common.RotateLeft(x4 + x7, 18);
                x11 ^= Common.RotateLeft(x10 + x9, 7);
                x8 ^= Common.RotateLeft(x11 + x10, 9);
                x9 ^= Common.RotateLeft(x8 + x11, 13);
                x10 ^= Common.RotateLeft(x9 + x8, 18);
                x12 ^= Common.RotateLeft(x15 + x14, 7);
                x13 ^= Common.RotateLeft(x12 + x15, 9);
                x14 ^= Common.RotateLeft(x13 + x12, 13);
                x15 ^= Common.RotateLeft(x14 + x13, 18);
            }

            Common.Store(output, 0, x0);
            Common.Store(output, 4, x5);
            Common.Store(output, 8, x10);
            Common.Store(output, 12, x15);
            Common.Store(output, 16, x6);
            Common.Store(output, 20, x7);
            Common.Store(output, 24, x8);
            Common.Store(output, 28, x9);
        }
    }
}