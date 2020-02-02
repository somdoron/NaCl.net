using System;
using System.Runtime.CompilerServices;

namespace NaCl.Internal
{
    internal static class StreamSalsa20
    {
        public const int KeyLength = 32;

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public void Transform(Span<byte> c, ReadOnlySpan<byte> n, ReadOnlySpan<byte> k)
        {
            Span<byte> input = new byte[16];
            Span<byte> block = new byte[64];
            Span<byte> kcopy = new byte[32];

            if (c.IsEmpty)
                return;

            for (int i = 0; i < 32; i++)
                kcopy[i] = k[i];
            
            for (int i = 0; i < 8; i++)
                input[i] = n[i];
            
            for (int i = 8; i < 16; i++)
                input[i] = 0;
            
            while (c.Length >= 64)
            {
                Salsa20.Transform(c, input, kcopy, Span<byte>.Empty);
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
                Salsa20.Transform(block, input, kcopy, Span<byte>.Empty);
                for (int i = 0; i < c.Length; i++)
                    c[i] = block[i];
            }

            block.Clear();
            kcopy.Clear();
        }
    }
}