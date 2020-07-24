using System;
using System.Runtime.CompilerServices;
using NetMQ.Utils;

namespace NaCl.Internal
{
    internal static class StreamSalsa20Xor
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Transform(Span<byte> c, ReadOnlySpan<byte> m, ReadOnlySpan<byte> n, ReadOnlySpan<byte> k,
            UInt64 ic = 0)
        {
            if (m.Length == 0)
                return;

            var input = new byte[16];
            var block = new byte[64];
            var kcopy = k.Slice(0, 32).ToArray();
            try
            {
                UInt32 u;

                n.Slice(0, 8).CopyTo(input);

                for (int i = 8; i < 16; i++)
                {
                    input[i] = (byte) (ic & 0xff);
                    ic >>= 8;
                }

                int mlen = m.Length;

                while (mlen >= 64)
                {
                    Salsa20.Transform(block, input, kcopy, null);

                    for (int i = 0; i < 64; i++)
                    {
                        c[i] = (byte)(m[i] ^ block[i]);
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
                    Salsa20.Transform(block, input, kcopy, null);
                    for (int i = 0; i < mlen; i++)
                    {
                        c[i] = (byte) (m[i] ^ block[i]);
                    }
                }
            }
            finally
            {
                input.Clear();
                block.Clear();
                kcopy.Clear();
            }
        }
    }
}