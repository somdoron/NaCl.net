using System;
using System.Runtime.CompilerServices;
using NetMQ.Utils;

namespace NaCl.Internal
{
    internal static class StreamSalsa20
    {
        public const int KeyLength = 32;

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public void Transform(Span<byte> c, ReadOnlySpan<byte> n, ReadOnlySpan<byte> k)
        {
            if (c.IsEmpty)
                return;

            var input = new byte[16];
            var block = new byte[64];
            var kcopy = k.ToArray();
            try
            {
                for (int i = 0; i < 8; i++)
                    input[i] = n[i];
                
                while (c.Length >= 64)
                {
                    Salsa20.Transform(block, input, kcopy, null);

                    block.CopyTo(c);

                    uint u = 1;
                    for (int i = 8; i < 16; i++)
                    {
                        u += (uint) input[i];
                        input[i] = (byte) u;
                        u >>= 8;
                    }

                    c = c.Slice(64);
                }

                if (c.Length != 0)
                {
                    Salsa20.Transform(block, input, kcopy, null);
                    for (int i = 0; i < c.Length; i++)
                        c[i] = block[i];
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