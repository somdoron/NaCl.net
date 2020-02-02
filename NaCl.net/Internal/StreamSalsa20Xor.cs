using System;
using System.Runtime.CompilerServices;

namespace NaCl.Internal
{
    internal static class StreamSalsa20Xor
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Transform(Span<byte> c, ReadOnlySpan<byte> m, ReadOnlySpan<byte> n, ReadOnlySpan<byte> k,
            UInt64 ic = 0)
        {
            Span<byte> input = stackalloc byte[16];
            Span<byte> block = stackalloc byte[64];
            Span<byte> kcopy = stackalloc byte[32];
            UInt32 u;

            if (m.Length == 0)
                return;

            for (int i = 0; i < 32; i++)
            {
                kcopy[i] = k[i];
            }

            for (int i = 0; i < 8; i++)
            {
                input[i] = n[i];
            }

            for (int i = 8; i < 16; i++)
            {
                input[i] = (byte) (ic & 0xff);
                ic >>= 8;
            }

            int mlen = m.Length;

            while (mlen >= 64)
            {
                Salsa20.Transform(block, input, kcopy, ReadOnlySpan<byte>.Empty);
                for (int i = 0; i < 64; i++)
                {
                    c[i] = (byte) (m[i] ^ block[i]);
                }

                u = 1;
                for (int i = 8; i < 16; i++)
                {
                    u += input[i];
                    input[i] = (byte) u;
                    u >>= 8;
                }

                mlen -= 64;
                c = c.Slice(64);
                m = m.Slice(64);
            }

            if (mlen != 0)
            {
                Salsa20.Transform(block, input, kcopy, ReadOnlySpan<byte>.Empty);
                for (int i = 0; i < mlen; i++)
                {
                    c[i] = (byte) (m[i] ^ block[i]);
                }
            }

            block.Clear();
            kcopy.Clear();
        }
    }
}