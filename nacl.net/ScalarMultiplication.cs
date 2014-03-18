using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nacl
{
    public static class ScalarMultiplication
    {
        public const int Bytes = 32;

        public const int ScalarBytes = 32;

        

        

        

        public static void MultiplyBase(byte[] q, byte[] n)
        {
            Multiply(q, n, Constants.Base);
        }

        public static void Multiply(byte[] q, byte[] n, byte[] p)
        {
            ArraySegment<uint> work = new uint[96];
            byte[] e = new byte[32];
            uint i;
            for (i = 0; i < 32; ++i) e[i] = n[i];
            e[0] &= 248;
            e[31] &= 127;
            e[31] |= 64;
            for (i = 0; i < 32; ++i) work[i] = p[i];
            MainLoop(work, e);
            Recip(work + 32, work + 32);
            Mult(work + 64, work, work + 32);
            Freeze(work + 64);
            for (i = 0; i < 32; ++i) 
                q[i] = (byte)work[64 + i];            
        }

        private static void Add(ArraySegment<uint> output, ArraySegment<uint> a, ArraySegment<uint> b)
        {
            uint j;
            uint u;
            u = 0;
            for (j = 0; j < 31; ++j)
            {
                u += a[j] + b[j]; output[j] = u & 255; u >>= 8;
            }
            u += a[31] + b[31];
            output[31] = u;
        }

        private static void Sub(ArraySegment<uint> output, ArraySegment<uint> a, ArraySegment<uint> b)
        {
            uint j;
            uint u;
            u = 218;
            for (j = 0; j < 31; ++j)
            {
                u += a[j] + 65280 - b[j];
                output[j] = u & 255;
                u >>= 8;
            }
            u += a[31] - b[31];
            output[31] = u;
        }

        private static void Squeeze(ArraySegment<uint> a)
        {
            uint j;
            uint u;
            u = 0;
            for (j = 0; j < 31; ++j) { u += a[j]; a[j] = u & 255; u >>= 8; }
            u += a[31]; a[31] = u & 127;
            u = 19 * (u >> 7);
            for (j = 0; j < 31; ++j) { u += a[j]; a[j] = u & 255; u >>= 8; }
            u += a[31]; a[31] = u;
        }

        private static void Freeze(ArraySegment<uint> a)
        {
            ArraySegment<uint> aorig = new uint[32];
            uint j;
            uint negative;

            for (j = 0; j < 32; ++j)
                aorig[j] = a[j];

            Add(a, a, Constants.MinUsp);
            negative = (uint)(-((a[31] >> 7) & 1));
            for (j = 0; j < 32; ++j)
                a[j] ^= negative & (aorig[j] ^ a[j]);
        }

        private static void Mult(ArraySegment<uint> output, ArraySegment<uint> a, ArraySegment<uint> b)
        {
            uint i;
            uint j;
            uint u;

            for (i = 0; i < 32; ++i)
            {
                u = 0;
                for (j = 0; j <= i; ++j) u += a[j] * b[i - j];
                for (j = i + 1; j < 32; ++j) u += 38 * a[j] * b[i + 32 - j];
                output[i] = u;
            }
            Squeeze(output);
        }

        static void Mult121665(ArraySegment<uint> output, ArraySegment<uint> a)
        {
            uint j;
            uint u;

            u = 0;
            for (j = 0; j < 31; ++j)
            {
                u += 121665 * a[j];
                output[j] = u & 255; u >>= 8;
            }
            u += 121665 * a[31];
            output[31] = u & 127;
            u = 19 * (u >> 7);
            for (j = 0; j < 31; ++j)
            { u += output[j]; output[j] = u & 255; u >>= 8; }
            u += output[j];
            output[j] = u;
        }

        private static void Square(ArraySegment<uint> output, ArraySegment<uint> a)
        {
            uint i;
            uint j;
            uint u;

            for (i = 0; i < 32; ++i)
            {
                u = 0;
                for (j = 0; j < i - j; ++j) u += a[j] * a[i - j];
                for (j = i + 1; j < i + 32 - j; ++j) u += 38 * a[j] * a[i + 32 - j];
                u *= 2;
                if ((i & 1) == 0)
                {
                    u += a[i / 2] * a[i / 2];
                    u += 38 * a[i / 2 + 16] * a[i / 2 + 16];
                }
                output[i] = u;
            }
            Squeeze(output);
        }

        static void Select(ArraySegment<uint> p, ArraySegment<uint> q, ArraySegment<uint> r, ArraySegment<uint> s, uint b)
        {
            uint j;
            uint t;
            uint bminus1;

            bminus1 = b - 1;
            for (j = 0; j < 64; ++j)
            {
                t = bminus1 & (r[j] ^ s[j]);
                p[j] = s[j] ^ t;
                q[j] = r[j] ^ t;
            }
        }

        static void MainLoop(ArraySegment<uint> work, byte[] e)
        {
            ArraySegment<uint> xzm1 = new uint[64];
            ArraySegment<uint> xzm = new uint[64];
            ArraySegment<uint> xzmb = new uint[64];
            ArraySegment<uint> xzm1b = new uint[64];
            ArraySegment<uint> xznb = new uint[64];
            ArraySegment<uint> xzn1b = new uint[64];
            ArraySegment<uint> a0 = new uint[64];
            ArraySegment<uint> a1 = new uint[64];
            ArraySegment<uint> b0 = new uint[64];
            ArraySegment<uint> b1 = new uint[64];
            ArraySegment<uint> c1 = new uint[64];
            ArraySegment<uint> r = new uint[32]; ;
            ArraySegment<uint> s = new uint[32]; ;
            ArraySegment<uint> t = new uint[32]; ;
            ArraySegment<uint> u = new uint[32]; ;
            uint j;
            uint b;
            int pos;

            for (j = 0; j < 32; ++j) xzm1[j] = work[j];
            xzm1[32] = 1;
            for (j = 33; j < 64; ++j) xzm1[j] = 0;

            xzm[0] = 1;
            for (j = 1; j < 64; ++j) xzm[j] = 0;

            for (pos = 254; pos >= 0; --pos)
            {
                b = (uint)(e[pos / 8] >> (pos & 7));
                b &= 1;
                Select(xzmb, xzm1b, xzm, xzm1, b);
                Add(a0, xzmb, xzmb + 32);
                Sub(a0 + 32, xzmb, xzmb + 32);
                Add(a1, xzm1b, xzm1b + 32);
                Sub(a1 + 32, xzm1b, xzm1b + 32);
                Square(b0, a0);
                Square(b0 + 32, a0 + 32);
                Mult(b1, a1, a0 + 32);
                Mult(b1 + 32, a1 + 32, a0);
                Add(c1, b1, b1 + 32);
                Sub(c1 + 32, b1, b1 + 32);
                Square(r, c1 + 32);
                Sub(s, b0, b0 + 32);
                Mult121665(t, s);
                Add(u, t, b0);
                Mult(xznb, b0, b0 + 32);
                Mult(xznb + 32, s, u);
                Square(xzn1b, c1);
                Mult(xzn1b + 32, r, work);
                Select(xzm, xzm1, xznb, xzn1b, b);
            }

            for (j = 0; j < 64; ++j) work[j] = xzm[j];
        }

        private static void Recip(ArraySegment<uint> output, ArraySegment<uint> z)
        {
            ArraySegment<uint> z2 = new uint[32];
            ArraySegment<uint> z9 = new uint[32];
            ArraySegment<uint> z11 = new uint[32];
            ArraySegment<uint> z2_5_0 = new uint[32];
            ArraySegment<uint> z2_10_0 = new uint[32];
            ArraySegment<uint> z2_20_0 = new uint[32];
            ArraySegment<uint> z2_50_0 = new uint[32];
            ArraySegment<uint> z2_100_0 = new uint[32];
            ArraySegment<uint> t0 = new uint[32];
            ArraySegment<uint> t1 = new uint[32];
            int i;

            /* 2 */
            Square(z2, z);
            /* 4 */
            Square(t1, z2);
            /* 8 */
            Square(t0, t1);
            /* 9 */
            Mult(z9, t0, z);
            /* 11 */
            Mult(z11, z9, z2);
            /* 22 */
            Square(t0, z11);
            /* 2^5 - 2^0 = 31 */
            Mult(z2_5_0, t0, z9);

            /* 2^6 - 2^1 */
            Square(t0, z2_5_0);
            /* 2^7 - 2^2 */
            Square(t1, t0);
            /* 2^8 - 2^3 */
            Square(t0, t1);
            /* 2^9 - 2^4 */
            Square(t1, t0);
            /* 2^10 - 2^5 */
            Square(t0, t1);
            /* 2^10 - 2^0 */
            Mult(z2_10_0, t0, z2_5_0);

            /* 2^11 - 2^1 */
            Square(t0, z2_10_0);
            /* 2^12 - 2^2 */
            Square(t1, t0);
            /* 2^20 - 2^10 */
            for (i = 2; i < 10; i += 2) { Square(t0, t1); Square(t1, t0); }
            /* 2^20 - 2^0 */
            Mult(z2_20_0, t1, z2_10_0);

            /* 2^21 - 2^1 */
            Square(t0, z2_20_0);
            /* 2^22 - 2^2 */
            Square(t1, t0);
            /* 2^40 - 2^20 */
            for (i = 2; i < 20; i += 2) { Square(t0, t1); Square(t1, t0); }
            /* 2^40 - 2^0 */
            Mult(t0, t1, z2_20_0);

            /* 2^41 - 2^1 */
            Square(t1, t0);
            /* 2^42 - 2^2 */
            Square(t0, t1);
            /* 2^50 - 2^10 */
            for (i = 2; i < 10; i += 2) { Square(t1, t0); Square(t0, t1); }
            /* 2^50 - 2^0 */
            Mult(z2_50_0, t0, z2_10_0);

            /* 2^51 - 2^1 */
            Square(t0, z2_50_0);
            /* 2^52 - 2^2 */
            Square(t1, t0);
            /* 2^100 - 2^50 */
            for (i = 2; i < 50; i += 2) { Square(t0, t1); Square(t1, t0); }
            /* 2^100 - 2^0 */
            Mult(z2_100_0, t1, z2_50_0);

            /* 2^101 - 2^1 */
            Square(t1, z2_100_0);
            /* 2^102 - 2^2 */
            Square(t0, t1);
            /* 2^200 - 2^100 */
            for (i = 2; i < 100; i += 2) { Square(t1, t0); Square(t0, t1); }
            /* 2^200 - 2^0 */
            Mult(t1, t0, z2_100_0);

            /* 2^201 - 2^1 */
            Square(t0, t1);
            /* 2^202 - 2^2 */
            Square(t1, t0);
            /* 2^250 - 2^50 */
            for (i = 2; i < 50; i += 2) { Square(t0, t1); Square(t1, t0); }
            /* 2^250 - 2^0 */
            Mult(t0, t1, z2_50_0);

            /* 2^251 - 2^1 */
            Square(t1, t0);
            /* 2^252 - 2^2 */
            Square(t0, t1);
            /* 2^253 - 2^3 */
            Square(t1, t0);
            /* 2^254 - 2^4 */
            Square(t0, t1);
            /* 2^255 - 2^5 */
            Square(t1, t0);
            /* 2^255 - 21 */
            Mult(output, t1, z11);
        }
    }
}


