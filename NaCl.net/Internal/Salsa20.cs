using System;
using System.Runtime.CompilerServices;

namespace NaCl.Internal
{
    internal static class Salsa20
    {
        private const int Rounds = 20;
        public const int KeyLength = 32;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Transform(byte[] output, byte[] input, byte[] k,
            byte[] c)
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
            UInt32 j0;
            UInt32 j1;
            UInt32 j2;
            UInt32 j3;
            UInt32 j4;
            UInt32 j5;
            UInt32 j6;
            UInt32 j7;
            UInt32 j8;
            UInt32 j9;
            UInt32 j10;
            UInt32 j11;
            UInt32 j12;
            UInt32 j13;
            UInt32 j14;
            UInt32 j15;

            j0 = x0 = 0x61707865;
            j5 = x5 = 0x3320646e;
            j10 = x10 = 0x79622d32;
            j15 = x15 = 0x6b206574;
            if (c!= null)
            {
                j0 = x0 = Common.Load32(c, 0);
                j5 = x5 = Common.Load32(c, 4);
                j10 = x10 = Common.Load32(c, 8);
                j15 = x15 = Common.Load32(c, 12);
            }

            j1 = x1 = Common.Load32(k, 0);
            j2 = x2 = Common.Load32(k, 4);
            j3 = x3 = Common.Load32(k, 8);
            j4 = x4 = Common.Load32(k, 12);
            j11 = x11 = Common.Load32(k, 16);
            j12 = x12 = Common.Load32(k, 20);
            j13 = x13 = Common.Load32(k, 24);
            j14 = x14 = Common.Load32(k, 28);

            j6 = x6 = Common.Load32(input, 0);
            j7 = x7 = Common.Load32(input, 4);
            j8 = x8 = Common.Load32(input, 8);
            j9 = x9 = Common.Load32(input, 12);

            for (int i = 0; i < Rounds; i += 2)
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

            Common.Store(output, 0, x0 + j0);
            Common.Store(output, 4, x1 + j1);
            Common.Store(output, 8, x2 + j2);
            Common.Store(output, 12, x3 + j3);
            Common.Store(output, 16, x4 + j4);
            Common.Store(output, 20, x5 + j5);
            Common.Store(output, 24, x6 + j6);
            Common.Store(output, 28, x7 + j7);
            Common.Store(output, 32, x8 + j8);
            Common.Store(output, 36, x9 + j9);
            Common.Store(output, 40, x10 + j10);
            Common.Store(output, 44, x11 + j11);
            Common.Store(output, 48, x12 + j12);
            Common.Store(output, 52, x13 + j13);
            Common.Store(output, 56, x14 + j14);
            Common.Store(output, 60, x15 + j15);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Transform(Span<byte> output, ReadOnlySpan<byte> input, ReadOnlySpan<byte> k,
            ReadOnlySpan<byte> c)
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
            UInt32 j0;
            UInt32 j1;
            UInt32 j2;
            UInt32 j3;
            UInt32 j4;
            UInt32 j5;
            UInt32 j6;
            UInt32 j7;
            UInt32 j8;
            UInt32 j9;
            UInt32 j10;
            UInt32 j11;
            UInt32 j12;
            UInt32 j13;
            UInt32 j14;
            UInt32 j15;

            j0 = x0 = 0x61707865;
            j5 = x5 = 0x3320646e;
            j10 = x10 = 0x79622d32;
            j15 = x15 = 0x6b206574;
            if (!c.IsEmpty)
            {
                j0 = x0 = Common.Load32(c, 0);
                j5 = x5 = Common.Load32(c, 4);
                j10 = x10 = Common.Load32(c, 8);
                j15 = x15 = Common.Load32(c, 12);
            }

            j1 = x1 = Common.Load32(k, 0);
            j2 = x2 = Common.Load32(k, 4);
            j3 = x3 = Common.Load32(k, 8);
            j4 = x4 = Common.Load32(k, 12);
            j11 = x11 = Common.Load32(k, 16);
            j12 = x12 = Common.Load32(k, 20);
            j13 = x13 = Common.Load32(k, 24);
            j14 = x14 = Common.Load32(k, 28);

            j6 = x6 = Common.Load32(input, 0);
            j7 = x7 = Common.Load32(input, 4);
            j8 = x8 = Common.Load32(input, 8);
            j9 = x9 = Common.Load32(input, 12);

            for (int i = 0; i < Rounds; i += 2)
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

            Common.Store(output, 0, x0 + j0);
            Common.Store(output, 4, x1 + j1);
            Common.Store(output, 8, x2 + j2);
            Common.Store(output, 12, x3 + j3);
            Common.Store(output, 16, x4 + j4);
            Common.Store(output, 20, x5 + j5);
            Common.Store(output, 24, x6 + j6);
            Common.Store(output, 28, x7 + j7);
            Common.Store(output, 32, x8 + j8);
            Common.Store(output, 36, x9 + j9);
            Common.Store(output, 40, x10 + j10);
            Common.Store(output, 44, x11 + j11);
            Common.Store(output, 48, x12 + j12);
            Common.Store(output, 52, x13 + j13);
            Common.Store(output, 56, x14 + j14);
            Common.Store(output, 60, x15 + j15);
        }
    }
}