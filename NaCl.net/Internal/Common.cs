using System;
using System.Runtime.CompilerServices;

namespace NaCl.Internal
{
    internal static class Common
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 Load32(byte[] src, int offset)
        {
            UInt32 w = (UInt32) src[offset];
            w |= (UInt32) src[offset + 1] << 8;
            w |= (UInt32) src[offset + 2] << 16;
            w |= (UInt32) src[offset + 3] << 24;
            return w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 Load32(ReadOnlySpan<byte> src, int offset)
        {
            UInt32 w = (UInt32)src[offset];
            w |= (UInt32)src[offset + 1] << 8;
            w |= (UInt32)src[offset + 2] << 16;
            w |= (UInt32)src[offset + 3] << 24;
            return w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Store(byte[] dst, int offset, UInt32 w)
        {
            dst[offset] = (byte)w; w >>= 8;
            dst[offset + 1] = (byte)w; w >>= 8;
            dst[offset + 2] = (byte)w; w >>= 8;
            dst[offset + 3] = (byte)w;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Store(Span<byte> dst, int offset, UInt32 w)
        {
            dst[offset] = (byte) w; w >>= 8;
            dst[offset + 1] = (byte) w; w >>= 8;
            dst[offset + 2] = (byte) w; w >>= 8;
            dst[offset + 3] = (byte) w;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 RotateLeft(in UInt32 x, in int b)
        {
            return (x << b) | (x >> (32 - b));
        }
    }
}