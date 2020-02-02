using System;

namespace NaCl.Internal
{
    using bytes = System.Span<byte>;
    using fe25519 = System.Span<int>;

    internal static class Fe25519
    {
        public const int ArraySize = 10;

        public static void Zero(fe25519 h)
        {
            h.Slice(0, 10).Fill(0);
        }

        public static void One(fe25519 h)
        {
            h[0] = 1;
            h.Slice(1, 9).Fill(0);
        }

        public static void Copy(fe25519 h, fe25519 f)
        {
            int f0 = f[0];
            int f1 = f[1];
            int f2 = f[2];
            int f3 = f[3];
            int f4 = f[4];
            int f5 = f[5];
            int f6 = f[6];
            int f7 = f[7];
            int f8 = f[8];
            int f9 = f[9];

            h[0] = f0;
            h[1] = f1;
            h[2] = f2;
            h[3] = f3;
            h[4] = f4;
            h[5] = f5;
            h[6] = f6;
            h[7] = f7;
            h[8] = f8;
            h[9] = f9;
        }

        static Int64 Load3(ReadOnlySpan<byte> input)
        {
            var result = (Int64) input[0];
            result |= ((Int64) input[1]) << 8;
            result |= ((Int64) input[2]) << 16;

            return result;
        }

        static Int64 Load4(ReadOnlySpan<byte> input)
        {
            var result = (Int64) input[0];
            result |= ((Int64) input[1]) << 8;
            result |= ((Int64) input[2]) << 16;
            result |= ((Int64) input[3]) << 24;

            return result;
        }

        public static void FromBytes(fe25519 h, ReadOnlySpan<byte> s)
        {
            Int64 h0 = Load4(s);
            Int64 h1 = Load3(s.Slice(4)) << 6;
            Int64 h2 = Load3(s.Slice(7)) << 5;
            Int64 h3 = Load3(s.Slice(10)) << 3;
            Int64 h4 = Load3(s.Slice(13)) << 2;
            Int64 h5 = Load4(s.Slice(16));
            Int64 h6 = Load3(s.Slice(20)) << 7;
            Int64 h7 = Load3(s.Slice(23)) << 5;
            Int64 h8 = Load3(s.Slice(26)) << 4;
            Int64 h9 = (Load3(s.Slice(29)) & 8388607) << 2;

            Int64 carry0;
            Int64 carry1;
            Int64 carry2;
            Int64 carry3;
            Int64 carry4;
            Int64 carry5;
            Int64 carry6;
            Int64 carry7;
            Int64 carry8;
            Int64 carry9;

            carry9 = (h9 + (long) (1 << 24)) >> 25;
            h0 += carry9 * 19;
            h9 -= carry9 << 25;
            carry1 = (h1 + (long) (1 << 24)) >> 25;
            h2 += carry1;
            h1 -= carry1 << 25;
            carry3 = (h3 + (long) (1 << 24)) >> 25;
            h4 += carry3;
            h3 -= carry3 << 25;
            carry5 = (h5 + (long) (1 << 24)) >> 25;
            h6 += carry5;
            h5 -= carry5 << 25;
            carry7 = (h7 + (long) (1 << 24)) >> 25;
            h8 += carry7;
            h7 -= carry7 << 25;

            carry0 = (h0 + (long) (1 << 25)) >> 26;
            h1 += carry0;
            h0 -= carry0 << 26;
            carry2 = (h2 + (long) (1 << 25)) >> 26;
            h3 += carry2;
            h2 -= carry2 << 26;
            carry4 = (h4 + (long) (1 << 25)) >> 26;
            h5 += carry4;
            h4 -= carry4 << 26;
            carry6 = (h6 + (long) (1 << 25)) >> 26;
            h7 += carry6;
            h6 -= carry6 << 26;
            carry8 = (h8 + (long) (1 << 25)) >> 26;
            h9 += carry8;
            h8 -= carry8 << 26;

            h[0] = (int) h0;
            h[1] = (int) h1;
            h[2] = (int) h2;
            h[3] = (int) h3;
            h[4] = (int) h4;
            h[5] = (int) h5;
            h[6] = (int) h6;
            h[7] = (int) h7;
            h[8] = (int) h8;
            h[9] = (int) h9;
        }

        public static void CSwap(fe25519 f, fe25519 g, int b)
        {
            int mask = -b;

            int f0 = f[0];
            int f1 = f[1];
            int f2 = f[2];
            int f3 = f[3];
            int f4 = f[4];
            int f5 = f[5];
            int f6 = f[6];
            int f7 = f[7];
            int f8 = f[8];
            int f9 = f[9];

            int g0 = g[0];
            int g1 = g[1];
            int g2 = g[2];
            int g3 = g[3];
            int g4 = g[4];
            int g5 = g[5];
            int g6 = g[6];
            int g7 = g[7];
            int g8 = g[8];
            int g9 = g[9];

            int x0 = f0 ^ g0;
            int x1 = f1 ^ g1;
            int x2 = f2 ^ g2;
            int x3 = f3 ^ g3;
            int x4 = f4 ^ g4;
            int x5 = f5 ^ g5;
            int x6 = f6 ^ g6;
            int x7 = f7 ^ g7;
            int x8 = f8 ^ g8;
            int x9 = f9 ^ g9;

            x0 &= mask;
            x1 &= mask;
            x2 &= mask;
            x3 &= mask;
            x4 &= mask;
            x5 &= mask;
            x6 &= mask;
            x7 &= mask;
            x8 &= mask;
            x9 &= mask;

            f[0] = f0 ^ x0;
            f[1] = f1 ^ x1;
            f[2] = f2 ^ x2;
            f[3] = f3 ^ x3;
            f[4] = f4 ^ x4;
            f[5] = f5 ^ x5;
            f[6] = f6 ^ x6;
            f[7] = f7 ^ x7;
            f[8] = f8 ^ x8;
            f[9] = f9 ^ x9;

            g[0] = g0 ^ x0;
            g[1] = g1 ^ x1;
            g[2] = g2 ^ x2;
            g[3] = g3 ^ x3;
            g[4] = g4 ^ x4;
            g[5] = g5 ^ x5;
            g[6] = g6 ^ x6;
            g[7] = g7 ^ x7;
            g[8] = g8 ^ x8;
            g[9] = g9 ^ x9;
        }

        public static void Sub(fe25519 h, fe25519 f, fe25519 g)
        {
            int h0 = f[0] - g[0];
            int h1 = f[1] - g[1];
            int h2 = f[2] - g[2];
            int h3 = f[3] - g[3];
            int h4 = f[4] - g[4];
            int h5 = f[5] - g[5];
            int h6 = f[6] - g[6];
            int h7 = f[7] - g[7];
            int h8 = f[8] - g[8];
            int h9 = f[9] - g[9];

            h[0] = h0;
            h[1] = h1;
            h[2] = h2;
            h[3] = h3;
            h[4] = h4;
            h[5] = h5;
            h[6] = h6;
            h[7] = h7;
            h[8] = h8;
            h[9] = h9;
        }


        public static void Add(fe25519 h, fe25519 f, fe25519 g)
        {
            int h0 = f[0] + g[0];
            int h1 = f[1] + g[1];
            int h2 = f[2] + g[2];
            int h3 = f[3] + g[3];
            int h4 = f[4] + g[4];
            int h5 = f[5] + g[5];
            int h6 = f[6] + g[6];
            int h7 = f[7] + g[7];
            int h8 = f[8] + g[8];
            int h9 = f[9] + g[9];

            h[0] = h0;
            h[1] = h1;
            h[2] = h2;
            h[3] = h3;
            h[4] = h4;
            h[5] = h5;
            h[6] = h6;
            h[7] = h7;
            h[8] = h8;
            h[9] = h9;
        }

        public static void Mul(fe25519 h, fe25519 f, fe25519 g)
        {
            int f0 = f[0];
            int f1 = f[1];
            int f2 = f[2];
            int f3 = f[3];
            int f4 = f[4];
            int f5 = f[5];
            int f6 = f[6];
            int f7 = f[7];
            int f8 = f[8];
            int f9 = f[9];

            int g0 = g[0];
            int g1 = g[1];
            int g2 = g[2];
            int g3 = g[3];
            int g4 = g[4];
            int g5 = g[5];
            int g6 = g[6];
            int g7 = g[7];
            int g8 = g[8];
            int g9 = g[9];

            int g1_19 = 19 * g1; /* 1.959375*2^29 */
            int g2_19 = 19 * g2; /* 1.959375*2^30; still ok */
            int g3_19 = 19 * g3;
            int g4_19 = 19 * g4;
            int g5_19 = 19 * g5;
            int g6_19 = 19 * g6;
            int g7_19 = 19 * g7;
            int g8_19 = 19 * g8;
            int g9_19 = 19 * g9;
            int f1_2 = 2 * f1;
            int f3_2 = 2 * f3;
            int f5_2 = 2 * f5;
            int f7_2 = 2 * f7;
            int f9_2 = 2 * f9;

            long f0g0 = f0 * (long) g0;
            long f0g1 = f0 * (long) g1;
            long f0g2 = f0 * (long) g2;
            long f0g3 = f0 * (long) g3;
            long f0g4 = f0 * (long) g4;
            long f0g5 = f0 * (long) g5;
            long f0g6 = f0 * (long) g6;
            long f0g7 = f0 * (long) g7;
            long f0g8 = f0 * (long) g8;
            long f0g9 = f0 * (long) g9;
            long f1g0 = f1 * (long) g0;
            long f1g1_2 = f1_2 * (long) g1;
            long f1g2 = f1 * (long) g2;
            long f1g3_2 = f1_2 * (long) g3;
            long f1g4 = f1 * (long) g4;
            long f1g5_2 = f1_2 * (long) g5;
            long f1g6 = f1 * (long) g6;
            long f1g7_2 = f1_2 * (long) g7;
            long f1g8 = f1 * (long) g8;
            long f1g9_38 = f1_2 * (long) g9_19;
            long f2g0 = f2 * (long) g0;
            long f2g1 = f2 * (long) g1;
            long f2g2 = f2 * (long) g2;
            long f2g3 = f2 * (long) g3;
            long f2g4 = f2 * (long) g4;
            long f2g5 = f2 * (long) g5;
            long f2g6 = f2 * (long) g6;
            long f2g7 = f2 * (long) g7;
            long f2g8_19 = f2 * (long) g8_19;
            long f2g9_19 = f2 * (long) g9_19;
            long f3g0 = f3 * (long) g0;
            long f3g1_2 = f3_2 * (long) g1;
            long f3g2 = f3 * (long) g2;
            long f3g3_2 = f3_2 * (long) g3;
            long f3g4 = f3 * (long) g4;
            long f3g5_2 = f3_2 * (long) g5;
            long f3g6 = f3 * (long) g6;
            long f3g7_38 = f3_2 * (long) g7_19;
            long f3g8_19 = f3 * (long) g8_19;
            long f3g9_38 = f3_2 * (long) g9_19;
            long f4g0 = f4 * (long) g0;
            long f4g1 = f4 * (long) g1;
            long f4g2 = f4 * (long) g2;
            long f4g3 = f4 * (long) g3;
            long f4g4 = f4 * (long) g4;
            long f4g5 = f4 * (long) g5;
            long f4g6_19 = f4 * (long) g6_19;
            long f4g7_19 = f4 * (long) g7_19;
            long f4g8_19 = f4 * (long) g8_19;
            long f4g9_19 = f4 * (long) g9_19;
            long f5g0 = f5 * (long) g0;
            long f5g1_2 = f5_2 * (long) g1;
            long f5g2 = f5 * (long) g2;
            long f5g3_2 = f5_2 * (long) g3;
            long f5g4 = f5 * (long) g4;
            long f5g5_38 = f5_2 * (long) g5_19;
            long f5g6_19 = f5 * (long) g6_19;
            long f5g7_38 = f5_2 * (long) g7_19;
            long f5g8_19 = f5 * (long) g8_19;
            long f5g9_38 = f5_2 * (long) g9_19;
            long f6g0 = f6 * (long) g0;
            long f6g1 = f6 * (long) g1;
            long f6g2 = f6 * (long) g2;
            long f6g3 = f6 * (long) g3;
            long f6g4_19 = f6 * (long) g4_19;
            long f6g5_19 = f6 * (long) g5_19;
            long f6g6_19 = f6 * (long) g6_19;
            long f6g7_19 = f6 * (long) g7_19;
            long f6g8_19 = f6 * (long) g8_19;
            long f6g9_19 = f6 * (long) g9_19;
            long f7g0 = f7 * (long) g0;
            long f7g1_2 = f7_2 * (long) g1;
            long f7g2 = f7 * (long) g2;
            long f7g3_38 = f7_2 * (long) g3_19;
            long f7g4_19 = f7 * (long) g4_19;
            long f7g5_38 = f7_2 * (long) g5_19;
            long f7g6_19 = f7 * (long) g6_19;
            long f7g7_38 = f7_2 * (long) g7_19;
            long f7g8_19 = f7 * (long) g8_19;
            long f7g9_38 = f7_2 * (long) g9_19;
            long f8g0 = f8 * (long) g0;
            long f8g1 = f8 * (long) g1;
            long f8g2_19 = f8 * (long) g2_19;
            long f8g3_19 = f8 * (long) g3_19;
            long f8g4_19 = f8 * (long) g4_19;
            long f8g5_19 = f8 * (long) g5_19;
            long f8g6_19 = f8 * (long) g6_19;
            long f8g7_19 = f8 * (long) g7_19;
            long f8g8_19 = f8 * (long) g8_19;
            long f8g9_19 = f8 * (long) g9_19;
            long f9g0 = f9 * (long) g0;
            long f9g1_38 = f9_2 * (long) g1_19;
            long f9g2_19 = f9 * (long) g2_19;
            long f9g3_38 = f9_2 * (long) g3_19;
            long f9g4_19 = f9 * (long) g4_19;
            long f9g5_38 = f9_2 * (long) g5_19;
            long f9g6_19 = f9 * (long) g6_19;
            long f9g7_38 = f9_2 * (long) g7_19;
            long f9g8_19 = f9 * (long) g8_19;
            long f9g9_38 = f9_2 * (long) g9_19;

            long h0 = f0g0 + f1g9_38 + f2g8_19 + f3g7_38 + f4g6_19 + f5g5_38 +
                      f6g4_19 + f7g3_38 + f8g2_19 + f9g1_38;
            long h1 = f0g1 + f1g0 + f2g9_19 + f3g8_19 + f4g7_19 + f5g6_19 + f6g5_19 +
                      f7g4_19 + f8g3_19 + f9g2_19;
            long h2 = f0g2 + f1g1_2 + f2g0 + f3g9_38 + f4g8_19 + f5g7_38 + f6g6_19 +
                      f7g5_38 + f8g4_19 + f9g3_38;
            long h3 = f0g3 + f1g2 + f2g1 + f3g0 + f4g9_19 + f5g8_19 + f6g7_19 +
                      f7g6_19 + f8g5_19 + f9g4_19;
            long h4 = f0g4 + f1g3_2 + f2g2 + f3g1_2 + f4g0 + f5g9_38 + f6g8_19 +
                      f7g7_38 + f8g6_19 + f9g5_38;
            long h5 = f0g5 + f1g4 + f2g3 + f3g2 + f4g1 + f5g0 + f6g9_19 + f7g8_19 +
                      f8g7_19 + f9g6_19;
            long h6 = f0g6 + f1g5_2 + f2g4 + f3g3_2 + f4g2 + f5g1_2 + f6g0 +
                      f7g9_38 + f8g8_19 + f9g7_38;
            long h7 = f0g7 + f1g6 + f2g5 + f3g4 + f4g3 + f5g2 + f6g1 + f7g0 +
                      f8g9_19 + f9g8_19;
            long h8 = f0g8 + f1g7_2 + f2g6 + f3g5_2 + f4g4 + f5g3_2 + f6g2 + f7g1_2 +
                      f8g0 + f9g9_38;
            long h9 =
                f0g9 + f1g8 + f2g7 + f3g6 + f4g5 + f5g4 + f6g3 + f7g2 + f8g1 + f9g0;

            long carry0;
            long carry1;
            long carry2;
            long carry3;
            long carry4;
            long carry5;
            long carry6;
            long carry7;
            long carry8;
            long carry9;

            /*
             |h0| <= (1.65*1.65*2^52*(1+19+19+19+19)+1.65*1.65*2^50*(38+38+38+38+38))
             i.e. |h0| <= 1.4*2^60; narrower ranges for h2, h4, h6, h8
             |h1| <= (1.65*1.65*2^51*(1+1+19+19+19+19+19+19+19+19))
             i.e. |h1| <= 1.7*2^59; narrower ranges for h3, h5, h7, h9
             */

            carry0 = (h0 + (long) (1 << 25)) >> 26;
            h1 += carry0;
            h0 -= carry0 * (1 << 26);
            carry4 = (h4 + (long) (1 << 25)) >> 26;
            h5 += carry4;
            h4 -= carry4 * (1 << 26);
            /* |h0| <= 2^25 */
            /* |h4| <= 2^25 */
            /* |h1| <= 1.71*2^59 */
            /* |h5| <= 1.71*2^59 */

            carry1 = (h1 + (long) (1 << 24)) >> 25;
            h2 += carry1;
            h1 -= carry1 * (1 << 25);
            carry5 = (h5 + (long) (1 << 24)) >> 25;
            h6 += carry5;
            h5 -= carry5 * (1 << 25);
            /* |h1| <= 2^24; from now on fits into int32 */
            /* |h5| <= 2^24; from now on fits into int32 */
            /* |h2| <= 1.41*2^60 */
            /* |h6| <= 1.41*2^60 */

            carry2 = (h2 + (long) (1 << 25)) >> 26;
            h3 += carry2;
            h2 -= carry2 * (1 << 26);
            carry6 = (h6 + (long) (1 << 25)) >> 26;
            h7 += carry6;
            h6 -= carry6 * (1 << 26);
            /* |h2| <= 2^25; from now on fits into int32 unchanged */
            /* |h6| <= 2^25; from now on fits into int32 unchanged */
            /* |h3| <= 1.71*2^59 */
            /* |h7| <= 1.71*2^59 */

            carry3 = (h3 + (long) (1 << 24)) >> 25;
            h4 += carry3;
            h3 -= carry3 * (1 << 25);
            carry7 = (h7 + (long) (1 << 24)) >> 25;
            h8 += carry7;
            h7 -= carry7 * (1 << 25);
            /* |h3| <= 2^24; from now on fits into int32 unchanged */
            /* |h7| <= 2^24; from now on fits into int32 unchanged */
            /* |h4| <= 1.72*2^34 */
            /* |h8| <= 1.41*2^60 */

            carry4 = (h4 + (long) (1 << 25)) >> 26;
            h5 += carry4;
            h4 -= carry4 * (1 << 26);
            carry8 = (h8 + (long) (1 << 25)) >> 26;
            h9 += carry8;
            h8 -= carry8 * (1 << 26);
            /* |h4| <= 2^25; from now on fits into int32 unchanged */
            /* |h8| <= 2^25; from now on fits into int32 unchanged */
            /* |h5| <= 1.01*2^24 */
            /* |h9| <= 1.71*2^59 */

            carry9 = (h9 + (long) (1 << 24)) >> 25;
            h0 += carry9 * 19;
            h9 -= carry9 * (1 << 25);
            /* |h9| <= 2^24; from now on fits into int32 unchanged */
            /* |h0| <= 1.1*2^39 */

            carry0 = (h0 + (long) (1 << 25)) >> 26;
            h1 += carry0;
            h0 -= carry0 * (1 << 26);
            /* |h0| <= 2^25; from now on fits into int32 unchanged */
            /* |h1| <= 1.01*2^24 */

            h[0] = (int) h0;
            h[1] = (int) h1;
            h[2] = (int) h2;
            h[3] = (int) h3;
            h[4] = (int) h4;
            h[5] = (int) h5;
            h[6] = (int) h6;
            h[7] = (int) h7;
            h[8] = (int) h8;
            h[9] = (int) h9;
        }

        public static void Sq(fe25519 h, fe25519 f)
        {
            int f0 = f[0];
            int f1 = f[1];
            int f2 = f[2];
            int f3 = f[3];
            int f4 = f[4];
            int f5 = f[5];
            int f6 = f[6];
            int f7 = f[7];
            int f8 = f[8];
            int f9 = f[9];

            int f0_2 = 2 * f0;
            int f1_2 = 2 * f1;
            int f2_2 = 2 * f2;
            int f3_2 = 2 * f3;
            int f4_2 = 2 * f4;
            int f5_2 = 2 * f5;
            int f6_2 = 2 * f6;
            int f7_2 = 2 * f7;
            int f5_38 = 38 * f5; /* 1.959375*2^30 */
            int f6_19 = 19 * f6; /* 1.959375*2^30 */
            int f7_38 = 38 * f7; /* 1.959375*2^30 */
            int f8_19 = 19 * f8; /* 1.959375*2^30 */
            int f9_38 = 38 * f9; /* 1.959375*2^30 */

            long f0f0 = f0 * (long) f0;
            long f0f1_2 = f0_2 * (long) f1;
            long f0f2_2 = f0_2 * (long) f2;
            long f0f3_2 = f0_2 * (long) f3;
            long f0f4_2 = f0_2 * (long) f4;
            long f0f5_2 = f0_2 * (long) f5;
            long f0f6_2 = f0_2 * (long) f6;
            long f0f7_2 = f0_2 * (long) f7;
            long f0f8_2 = f0_2 * (long) f8;
            long f0f9_2 = f0_2 * (long) f9;
            long f1f1_2 = f1_2 * (long) f1;
            long f1f2_2 = f1_2 * (long) f2;
            long f1f3_4 = f1_2 * (long) f3_2;
            long f1f4_2 = f1_2 * (long) f4;
            long f1f5_4 = f1_2 * (long) f5_2;
            long f1f6_2 = f1_2 * (long) f6;
            long f1f7_4 = f1_2 * (long) f7_2;
            long f1f8_2 = f1_2 * (long) f8;
            long f1f9_76 = f1_2 * (long) f9_38;
            long f2f2 = f2 * (long) f2;
            long f2f3_2 = f2_2 * (long) f3;
            long f2f4_2 = f2_2 * (long) f4;
            long f2f5_2 = f2_2 * (long) f5;
            long f2f6_2 = f2_2 * (long) f6;
            long f2f7_2 = f2_2 * (long) f7;
            long f2f8_38 = f2_2 * (long) f8_19;
            long f2f9_38 = f2 * (long) f9_38;
            long f3f3_2 = f3_2 * (long) f3;
            long f3f4_2 = f3_2 * (long) f4;
            long f3f5_4 = f3_2 * (long) f5_2;
            long f3f6_2 = f3_2 * (long) f6;
            long f3f7_76 = f3_2 * (long) f7_38;
            long f3f8_38 = f3_2 * (long) f8_19;
            long f3f9_76 = f3_2 * (long) f9_38;
            long f4f4 = f4 * (long) f4;
            long f4f5_2 = f4_2 * (long) f5;
            long f4f6_38 = f4_2 * (long) f6_19;
            long f4f7_38 = f4 * (long) f7_38;
            long f4f8_38 = f4_2 * (long) f8_19;
            long f4f9_38 = f4 * (long) f9_38;
            long f5f5_38 = f5 * (long) f5_38;
            long f5f6_38 = f5_2 * (long) f6_19;
            long f5f7_76 = f5_2 * (long) f7_38;
            long f5f8_38 = f5_2 * (long) f8_19;
            long f5f9_76 = f5_2 * (long) f9_38;
            long f6f6_19 = f6 * (long) f6_19;
            long f6f7_38 = f6 * (long) f7_38;
            long f6f8_38 = f6_2 * (long) f8_19;
            long f6f9_38 = f6 * (long) f9_38;
            long f7f7_38 = f7 * (long) f7_38;
            long f7f8_38 = f7_2 * (long) f8_19;
            long f7f9_76 = f7_2 * (long) f9_38;
            long f8f8_19 = f8 * (long) f8_19;
            long f8f9_38 = f8 * (long) f9_38;
            long f9f9_38 = f9 * (long) f9_38;

            long h0 = f0f0 + f1f9_76 + f2f8_38 + f3f7_76 + f4f6_38 + f5f5_38;
            long h1 = f0f1_2 + f2f9_38 + f3f8_38 + f4f7_38 + f5f6_38;
            long h2 = f0f2_2 + f1f1_2 + f3f9_76 + f4f8_38 + f5f7_76 + f6f6_19;
            long h3 = f0f3_2 + f1f2_2 + f4f9_38 + f5f8_38 + f6f7_38;
            long h4 = f0f4_2 + f1f3_4 + f2f2 + f5f9_76 + f6f8_38 + f7f7_38;
            long h5 = f0f5_2 + f1f4_2 + f2f3_2 + f6f9_38 + f7f8_38;
            long h6 = f0f6_2 + f1f5_4 + f2f4_2 + f3f3_2 + f7f9_76 + f8f8_19;
            long h7 = f0f7_2 + f1f6_2 + f2f5_2 + f3f4_2 + f8f9_38;
            long h8 = f0f8_2 + f1f7_4 + f2f6_2 + f3f5_4 + f4f4 + f9f9_38;
            long h9 = f0f9_2 + f1f8_2 + f2f7_2 + f3f6_2 + f4f5_2;

            long carry0;
            long carry1;
            long carry2;
            long carry3;
            long carry4;
            long carry5;
            long carry6;
            long carry7;
            long carry8;
            long carry9;

            carry0 = (h0 + (long) (1 << 25)) >> 26;
            h1 += carry0;
            h0 -= carry0 * (1 << 26);
            carry4 = (h4 + (long) (1 << 25)) >> 26;
            h5 += carry4;
            h4 -= carry4 * (1 << 26);

            carry1 = (h1 + (long) (1 << 24)) >> 25;
            h2 += carry1;
            h1 -= carry1 * (1 << 25);
            carry5 = (h5 + (long) (1 << 24)) >> 25;
            h6 += carry5;
            h5 -= carry5 * (1 << 25);

            carry2 = (h2 + (long) (1 << 25)) >> 26;
            h3 += carry2;
            h2 -= carry2 * (1 << 26);
            carry6 = (h6 + (long) (1 << 25)) >> 26;
            h7 += carry6;
            h6 -= carry6 * (1 << 26);

            carry3 = (h3 + (long) (1 << 24)) >> 25;
            h4 += carry3;
            h3 -= carry3 * (1 << 25);
            carry7 = (h7 + (long) (1 << 24)) >> 25;
            h8 += carry7;
            h7 -= carry7 * (1 << 25);

            carry4 = (h4 + (long) (1 << 25)) >> 26;
            h5 += carry4;
            h4 -= carry4 * (1 << 26);
            carry8 = (h8 + (long) (1 << 25)) >> 26;
            h9 += carry8;
            h8 -= carry8 * (1 << 26);

            carry9 = (h9 + (long) (1 << 24)) >> 25;
            h0 += carry9 * 19;
            h9 -= carry9 * (1 << 25);

            carry0 = (h0 + (long) (1 << 25)) >> 26;
            h1 += carry0;
            h0 -= carry0 * (1 << 26);

            h[0] = (int) h0;
            h[1] = (int) h1;
            h[2] = (int) h2;
            h[3] = (int) h3;
            h[4] = (int) h4;
            h[5] = (int) h5;
            h[6] = (int) h6;
            h[7] = (int) h7;
            h[8] = (int) h8;
            h[9] = (int) h9;
        }

        public static void Mul121666(fe25519 h, fe25519 f)
        {
            int f0 = f[0];
            int f1 = f[1];
            int f2 = f[2];
            int f3 = f[3];
            int f4 = f[4];
            int f5 = f[5];
            int f6 = f[6];
            int f7 = f[7];
            int f8 = f[8];
            int f9 = f[9];
            long h0 = f0 * (long) 121666;
            long h1 = f1 * (long) 121666;
            long h2 = f2 * (long) 121666;
            long h3 = f3 * (long) 121666;
            long h4 = f4 * (long) 121666;
            long h5 = f5 * (long) 121666;
            long h6 = f6 * (long) 121666;
            long h7 = f7 * (long) 121666;
            long h8 = f8 * (long) 121666;
            long h9 = f9 * (long) 121666;
            long carry0;
            long carry1;
            long carry2;
            long carry3;
            long carry4;
            long carry5;
            long carry6;
            long carry7;
            long carry8;
            long carry9;

            carry9 = (h9 + (long) (1 << 24)) >> 25;
            h0 += carry9 * 19;
            h9 -= carry9 << 25;
            carry1 = (h1 + (long) (1 << 24)) >> 25;
            h2 += carry1;
            h1 -= carry1 << 25;
            carry3 = (h3 + (long) (1 << 24)) >> 25;
            h4 += carry3;
            h3 -= carry3 << 25;
            carry5 = (h5 + (long) (1 << 24)) >> 25;
            h6 += carry5;
            h5 -= carry5 << 25;
            carry7 = (h7 + (long) (1 << 24)) >> 25;
            h8 += carry7;
            h7 -= carry7 << 25;

            carry0 = (h0 + (long) (1 << 25)) >> 26;
            h1 += carry0;
            h0 -= carry0 << 26;
            carry2 = (h2 + (long) (1 << 25)) >> 26;
            h3 += carry2;
            h2 -= carry2 << 26;
            carry4 = (h4 + (long) (1 << 25)) >> 26;
            h5 += carry4;
            h4 -= carry4 << 26;
            carry6 = (h6 + (long) (1 << 25)) >> 26;
            h7 += carry6;
            h6 -= carry6 << 26;
            carry8 = (h8 + (long) (1 << 25)) >> 26;
            h9 += carry8;
            h8 -= carry8 << 26;

            h[0] = (int) h0;
            h[1] = (int) h1;
            h[2] = (int) h2;
            h[3] = (int) h3;
            h[4] = (int) h4;
            h[5] = (int) h5;
            h[6] = (int) h6;
            h[7] = (int) h7;
            h[8] = (int) h8;
            h[9] = (int) h9;
        }


        public static void Invert(fe25519 output, fe25519 z)
        {
            fe25519 t0 = stackalloc int[ArraySize];
            fe25519 t1 = stackalloc int[ArraySize];
            fe25519 t2 = stackalloc int[ArraySize];
            fe25519 t3 = stackalloc int[ArraySize];
            int i;

            Sq(t0, z);
            Sq(t1, t0);
            Sq(t1, t1);
            Mul(t1, z, t1);
            Mul(t0, t0, t1);
            Sq(t2, t0);
            Mul(t1, t1, t2);
            Sq(t2, t1);
            for (i = 1; i < 5; ++i)
            {
                Sq(t2, t2);
            }

            Mul(t1, t2, t1);
            Sq(t2, t1);
            for (i = 1; i < 10; ++i)
            {
                Sq(t2, t2);
            }

            Mul(t2, t2, t1);
            Sq(t3, t2);
            for (i = 1; i < 20; ++i)
            {
                Sq(t3, t3);
            }

            Mul(t2, t3, t2);
            Sq(t2, t2);
            for (i = 1; i < 10; ++i)
            {
                Sq(t2, t2);
            }

            Mul(t1, t2, t1);
            Sq(t2, t1);
            for (i = 1; i < 50; ++i)
            {
                Sq(t2, t2);
            }

            Mul(t2, t2, t1);
            Sq(t3, t2);
            for (i = 1; i < 100; ++i)
            {
                Sq(t3, t3);
            }

            Mul(t2, t3, t2);
            Sq(t2, t2);
            for (i = 1; i < 50; ++i)
            {
                Sq(t2, t2);
            }

            Mul(t1, t2, t1);
            Sq(t1, t1);
            for (i = 1; i < 5; ++i)
            {
                Sq(t1, t1);
            }

            Mul(output, t1, t0);
        }

        static void Reduce(fe25519 h, fe25519 f)
        {
            int h0 = f[0];
            int h1 = f[1];
            int h2 = f[2];
            int h3 = f[3];
            int h4 = f[4];
            int h5 = f[5];
            int h6 = f[6];
            int h7 = f[7];
            int h8 = f[8];
            int h9 = f[9];

            int q;
            int carry0, carry1, carry2, carry3, carry4, carry5, carry6, carry7, carry8, carry9;

            q = (19 * h9 + ((int) 1 << 24)) >> 25;
            q = (h0 + q) >> 26;
            q = (h1 + q) >> 25;
            q = (h2 + q) >> 26;
            q = (h3 + q) >> 25;
            q = (h4 + q) >> 26;
            q = (h5 + q) >> 25;
            q = (h6 + q) >> 26;
            q = (h7 + q) >> 25;
            q = (h8 + q) >> 26;
            q = (h9 + q) >> 25;

            /* Goal: Output h-(2^255-19)q, which is between 0 and 2^255-20. */
            h0 += 19 * q;
            /* Goal: Output h-2^255 q, which is between 0 and 2^255-20. */

            carry0 = h0 >> 26;
            h1 += carry0;
            h0 -= carry0 * (1 << 26);
            carry1 = h1 >> 25;
            h2 += carry1;
            h1 -= carry1 * (1 << 25);
            carry2 = h2 >> 26;
            h3 += carry2;
            h2 -= carry2 * (1 << 26);
            carry3 = h3 >> 25;
            h4 += carry3;
            h3 -= carry3 * (1 << 25);
            carry4 = h4 >> 26;
            h5 += carry4;
            h4 -= carry4 * (1 << 26);
            carry5 = h5 >> 25;
            h6 += carry5;
            h5 -= carry5 * (1 << 25);
            carry6 = h6 >> 26;
            h7 += carry6;
            h6 -= carry6 * (1 << 26);
            carry7 = h7 >> 25;
            h8 += carry7;
            h7 -= carry7 * (1 << 25);
            carry8 = h8 >> 26;
            h9 += carry8;
            h8 -= carry8 * (1 << 26);
            carry9 = h9 >> 25;
            h9 -= carry9 * (1 << 25);

            h[0] = h0;
            h[1] = h1;
            h[2] = h2;
            h[3] = h3;
            h[4] = h4;
            h[5] = h5;
            h[6] = h6;
            h[7] = h7;
            h[8] = h8;
            h[9] = h9;
        }


        public static void ToBytes(bytes s, fe25519 h)
        {
            fe25519 t = stackalloc int[ArraySize];

            Reduce(t, h);
            s[0] = (byte)(t[0] >> 0);
            s[1] = (byte)(t[0] >> 8);
            s[2] = (byte)(t[0] >> 16);
            s[3] = (byte)((t[0] >> 24) | (t[1] * (1 << 2)));
            s[4] = (byte)(t[1] >> 6);
            s[5] = (byte)(t[1] >> 14);
            s[6] = (byte)((t[1] >> 22) | (t[2] * (1 << 3)));
            s[7] = (byte)(t[2] >> 5);
            s[8] = (byte)(t[2] >> 13);
            s[9] = (byte)((t[2] >> 21) | (t[3] * (1 << 5)));
            s[10] = (byte)(t[3] >> 3);
            s[11] = (byte)(t[3] >> 11);
            s[12] = (byte)((t[3] >> 19) | (t[4] * (1 << 6)));
            s[13] = (byte)(t[4] >> 2);
            s[14] = (byte)(t[4] >> 10);
            s[15] = (byte)(t[4] >> 18);
            s[16] = (byte)(t[5] >> 0);
            s[17] = (byte)(t[5] >> 8);
            s[18] = (byte)(t[5] >> 16);
            s[19] = (byte)((t[5] >> 24) | (t[6] * (1 << 1)));
            s[20] = (byte)(t[6] >> 7);
            s[21] = (byte)(t[6] >> 15);
            s[22] = (byte)((t[6] >> 23) | (t[7] * (1 << 3)));
            s[23] = (byte)(t[7] >> 5);
            s[24] = (byte)(t[7] >> 13);
            s[25] = (byte)((t[7] >> 21) | (t[8] * (1 << 4)));
            s[26] = (byte)(t[8] >> 4);
            s[27] = (byte)(t[8] >> 12);
            s[28] = (byte)((t[8] >> 20) | (t[9] * (1 << 6)));
            s[29] = (byte)(t[9] >> 2);
            s[30] = (byte)(t[9] >> 10);
            s[31] = (byte)(t[9] >> 18);
        }
    }
}