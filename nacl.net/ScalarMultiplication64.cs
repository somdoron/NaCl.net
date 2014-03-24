using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nacl
{
  public static class ScalarMultiplication64
  {
    public static void MultiplyBase(byte[] q, byte[] n)
    {
      unsafe
      {
        byte* p = stackalloc byte[32];
        p[0] = 9;

        fixed (byte* qPointer = q, nPointer = n)
        {
          InternalMultiply(qPointer, nPointer, p);
        }
      }
    }

    public static void Multiply(byte[] q, byte[] n, byte[] p)
    {
      unsafe
      {
        fixed (byte* qPointer = q, nPointer = n, pPointer = p)
        {
          InternalMultiply(qPointer, nPointer, pPointer);
        }
      }
    }

    private static unsafe void InternalMultiply(byte* mypublic, byte* secret, byte* basepoint)
    {
      UInt64* bp = stackalloc UInt64[5];
      UInt64* x = stackalloc UInt64[5]; ;
      UInt64* z = stackalloc UInt64[5];
      UInt64* zmone = stackalloc UInt64[5];
      byte* e = stackalloc byte[32];
      int i;

      for (i = 0; i < 32; ++i) e[i] = secret[i];
      e[0] &= 248;
      e[31] &= 127;
      e[31] |= 64;

      fexpand(bp, basepoint);
      cmult(x, z, e, bp);
      crecip(zmone, z);
      fmul(z, x, zmone);
      fcontract(mypublic, z);
    }

    static unsafe void arrcpy(UInt64* dest, UInt64* source, int size)
    {
      for (int i = 0; i < size; i++)
      {
        dest[i] = source[i];
      }
    }

    static unsafe void fsum(UInt64* output, UInt64* input)
    {
      output[0] += input[0];
      output[1] += input[1];
      output[2] += input[2];
      output[3] += input[3];
      output[4] += input[4];
    }

    static unsafe void fdifference_backwards(UInt64* output, UInt64* input)
    {
      /* 152 is 19 << 3 */
      UInt64 two54m152 = (((UInt64)1) << 54) - 152;
      UInt64 two54m8 = (((UInt64)1) << 54) - 8;

      output[0] = input[0] + two54m152 - output[0];
      output[1] = input[1] + two54m8 - output[1];
      output[2] = input[2] + two54m8 - output[2];
      output[3] = input[3] + two54m8 - output[3];
      output[4] = input[4] + two54m8 - output[4];
    }

    static unsafe void fscalar_product(UInt64* output, UInt64* input, UInt64 scalar)
    {
      UInt128 a;

      a = UInt128.MultiplyUInt64(input[0], scalar);
      output[0] = ((UInt64)a) & 0x7ffffffffffff;

      a = UInt128.MultiplyUInt64(input[1], scalar) + ((UInt64)(a >> 51));
      output[1] = ((UInt64)a) & 0x7ffffffffffff;

      a = UInt128.MultiplyUInt64(input[2], scalar) + ((UInt64)(a >> 51));
      output[2] = ((UInt64)a) & 0x7ffffffffffff;

      a = UInt128.MultiplyUInt64(input[3], scalar) + ((UInt64)(a >> 51));
      output[3] = ((UInt64)a) & 0x7ffffffffffff;

      a = UInt128.MultiplyUInt64(input[4], scalar) + ((UInt64)(a >> 51));
      output[4] = ((UInt64)a) & 0x7ffffffffffff;

      output[0] += (UInt64)((a >> 51) * 19);
    }

    static unsafe void fmul(UInt64* output, UInt64* in2, UInt64* input)
    {
      UInt128* t = stackalloc UInt128[5];
      UInt64 r0, r1, r2, r3, r4, s0, s1, s2, s3, s4, c;

      r0 = input[0];
      r1 = input[1];
      r2 = input[2];
      r3 = input[3];
      r4 = input[4];

      s0 = in2[0];
      s1 = in2[1];
      s2 = in2[2];
      s3 = in2[3];
      s4 = in2[4];

      t[0] = UInt128.MultiplyUInt64(r0, s0);

      t[1] = UInt128.MultiplyUInt64(r0, s1);
      t[1].Add(UInt128.MultiplyUInt64(r1, s0));

      t[2] = UInt128.MultiplyUInt64(r0, s2);
      t[2].Add(UInt128.MultiplyUInt64(r2, s0));
      t[2].Add(UInt128.MultiplyUInt64(r1, s1));

      t[3] = UInt128.MultiplyUInt64(r0, s3);
      t[3].Add(UInt128.MultiplyUInt64(r3, s0));
      t[3].Add(UInt128.MultiplyUInt64(r1, s2));
      t[3].Add(UInt128.MultiplyUInt64(r2, s1));

      t[4] = UInt128.MultiplyUInt64(r0, s4);
      t[4].Add(UInt128.MultiplyUInt64(r4, s0));
      t[4].Add(UInt128.MultiplyUInt64(r3, s1));
      t[4].Add(UInt128.MultiplyUInt64(r1, s3));
      t[4].Add(UInt128.MultiplyUInt64(r2, s2));

      r4 *= 19;
      r1 *= 19;
      r2 *= 19;
      r3 *= 19;

      t[0].Add(UInt128.MultiplyUInt64(r4, s1));
      t[0].Add(UInt128.MultiplyUInt64(r1, s4));
      t[0].Add(UInt128.MultiplyUInt64(r2, s3));
      t[0].Add(UInt128.MultiplyUInt64(r3, s2));

      t[1].Add(UInt128.MultiplyUInt64(r4, s2));
      t[1].Add(UInt128.MultiplyUInt64(r2, s4));
      t[1].Add(UInt128.MultiplyUInt64(r3, s3));

      t[2].Add(UInt128.MultiplyUInt64(r4, s3));
      t[2].Add(UInt128.MultiplyUInt64(r3, s4));
      t[3].Add(UInt128.MultiplyUInt64(r4, s4));

      r0 = (UInt64)t[0] & 0x7ffffffffffff; 
      c = (UInt64)(t[0] >> 51);
      t[1] += c; 
      r1 = (UInt64)t[1] & 0x7ffffffffffff; 
      c = (UInt64)(t[1] >> 51);
      t[2] += c; 
      r2 = (UInt64)t[2] & 0x7ffffffffffff; 
      c = (UInt64)(t[2] >> 51);
      t[3] += c; 
      r3 = (UInt64)t[3] & 0x7ffffffffffff; 
      c = (UInt64)(t[3] >> 51);
      t[4] += c;
      r4 = (UInt64)t[4] & 0x7ffffffffffff; 
      c = (UInt64)(t[4] >> 51);
      r0 += c * 19; 
      c = r0 >> 51;
      r0 = r0 & 0x7ffffffffffff;
      r1 += c; 
      c = r1 >> 51; 
      r1 = r1 & 0x7ffffffffffff;
      r2 += c;

      output[0] = r0;
      output[1] = r1;
      output[2] = r2;
      output[3] = r3;
      output[4] = r4;
    }

    static unsafe void fsquare_times(UInt64* output, UInt64* input, UInt64 count)
    {
      UInt128* t = stackalloc UInt128[5];
      UInt64 r0, r1, r2, r3, r4, c;
      UInt64 d0, d1, d2, d4, d419;

      r0 = input[0];
      r1 = input[1];
      r2 = input[2];
      r3 = input[3];
      r4 = input[4];

      do
      {
        d0 = r0 * 2;
        d1 = r1 * 2;
        d2 = r2 * 2 * 19;
        d419 = r4 * 19;
        d4 = d419 * 2;

        t[0] = UInt128.MultiplyUInt64(r0, r0);
        t[0].Add(UInt128.MultiplyUInt64(d4, r1));
        t[0].Add(UInt128.MultiplyUInt64(d2, r3));

        t[1] = UInt128.MultiplyUInt64(d0, r1);
        t[1].Add(UInt128.MultiplyUInt64(d4, r2));
        t[1].Add(UInt128.MultiplyUInt64(r3, r3 * 19));

        t[2] = UInt128.MultiplyUInt64(d0, r2);
        t[2].Add(UInt128.MultiplyUInt64(r1, r1));
        t[2].Add(UInt128.MultiplyUInt64(d4, r3));

        t[3] = UInt128.MultiplyUInt64(d0, r3);
        t[3].Add(UInt128.MultiplyUInt64(d1, r2));
        t[3].Add(UInt128.MultiplyUInt64(r4, d419));

        t[4] = UInt128.MultiplyUInt64(d0, r4);
        t[4].Add(UInt128.MultiplyUInt64(d1, r3));
        t[4].Add(UInt128.MultiplyUInt64(r2, r2));

        r0 = (UInt64)t[0] & 0x7ffffffffffff; c = (UInt64)(t[0] >> 51);

        t[1].Add(c);
        r1 = (UInt64)t[1] & 0x7ffffffffffff;
        c = (UInt64)(t[1] >> 51);

        t[2].Add(c);
        r2 = (UInt64)t[2] & 0x7ffffffffffff; c = (UInt64)(t[2] >> 51);

        t[3].Add(c);
        r3 = (UInt64)t[3] & 0x7ffffffffffff;
        c = (UInt64)(t[3] >> 51);

        t[4].Add(c); 
        r4 = (UInt64)t[4] & 0x7ffffffffffff;
        c = (UInt64)(t[4] >> 51);
        r0 += c * 19; c = r0 >> 51; r0 = r0 & 0x7ffffffffffff;
        r1 += c; c = r1 >> 51; r1 = r1 & 0x7ffffffffffff;
        r2 += c;
      } while ((--count) != 0);

      output[0] = r0;
      output[1] = r1;
      output[2] = r2;
      output[3] = r3;
      output[4] = r4;
    }

    static unsafe void fexpand(UInt64* output, byte* input)
    {
      output[0] = *((UInt64*)(input)) & 0x7ffffffffffff;
      output[1] = (*((UInt64*)(input + 6)) >> 3) & 0x7ffffffffffff;
      output[2] = (*((UInt64*)(input + 12)) >> 6) & 0x7ffffffffffff;
      output[3] = (*((UInt64*)(input + 19)) >> 1) & 0x7ffffffffffff;
      output[4] = (*((UInt64*)(input + 25)) >> 4) & 0xfffffffffffff;
    }

    static unsafe void fcontract(byte* output, UInt64* input)
    {
      UInt128* t = stackalloc UInt128[5];

      t[0] = (UInt128)input[0];
      t[1] = (UInt128)input[1];
      t[2] = (UInt128)input[2];
      t[3] = (UInt128)input[3];
      t[4] = (UInt128)input[4];

      t[1] += t[0] >> 51; t[0] &= 0x7ffffffffffff;
      t[2] += t[1] >> 51; t[1] &= 0x7ffffffffffff;
      t[3] += t[2] >> 51; t[2] &= 0x7ffffffffffff;
      t[4] += t[3] >> 51; t[3] &= 0x7ffffffffffff;
      t[0] += (t[4] >> 51) * 19; t[4] &= 0x7ffffffffffff;

      t[1] += t[0] >> 51; t[0] &= 0x7ffffffffffff;
      t[2] += t[1] >> 51; t[1] &= 0x7ffffffffffff;
      t[3] += t[2] >> 51; t[2] &= 0x7ffffffffffff;
      t[4] += t[3] >> 51; t[3] &= 0x7ffffffffffff;
      t[0] += (t[4] >> 51) * 19; t[4] &= 0x7ffffffffffff;

      /* now t is between 0 and 2^255-1, properly carried. */
      /* case 1: between 0 and 2^255-20. case 2: between 2^255-19 and 2^255-1. */

      t[0] += 19;

      t[1] += t[0] >> 51; t[0] &= 0x7ffffffffffff;
      t[2] += t[1] >> 51; t[1] &= 0x7ffffffffffff;
      t[3] += t[2] >> 51; t[2] &= 0x7ffffffffffff;
      t[4] += t[3] >> 51; t[3] &= 0x7ffffffffffff;
      t[0] += (t[4] >> 51) * 19; t[4] &= 0x7ffffffffffff;

      /* now between 19 and 2^255-1 in both cases, and offset by 19. */

      t[0] += 0x8000000000000 - 19;
      t[1] += 0x8000000000000 - 1;
      t[2] += 0x8000000000000 - 1;
      t[3] += 0x8000000000000 - 1;
      t[4] += 0x8000000000000 - 1;

      /* now between 2^255 and 2^256-20, and offset by 2^255. */

      t[1] += t[0] >> 51; t[0] &= 0x7ffffffffffff;
      t[2] += t[1] >> 51; t[1] &= 0x7ffffffffffff;
      t[3] += t[2] >> 51; t[2] &= 0x7ffffffffffff;
      t[4] += t[3] >> 51; t[3] &= 0x7ffffffffffff;
      t[4] &= 0x7ffffffffffff;

      *((UInt64*)(output)) = (UInt64)(t[0] | (t[1] << 51));
      *((UInt64*)(output + 8)) = (UInt64)((t[1] >> 13) | (t[2] << 38));
      *((UInt64*)(output + 16)) = (UInt64)((t[2] >> 26) | (t[3] << 25));
      *((UInt64*)(output + 24)) = (UInt64)((t[3] >> 39) | (t[4] << 12));
    }

    static unsafe void fmonty(UInt64* x2, UInt64* z2, /* output 2Q */
       UInt64* x3, UInt64* z3, /* output Q + Q' */
       UInt64* x, UInt64* z,   /* input Q */
       UInt64* xprime, UInt64* zprime, /* input Q' */
       UInt64* qmqp /* input Q - Q' */)
    {
      UInt64* origx = stackalloc UInt64[5];
      UInt64* origxprime = stackalloc UInt64[5];
      UInt64* zzz = stackalloc UInt64[5];
      UInt64* xx = stackalloc UInt64[5];
      UInt64* zz = stackalloc UInt64[5];
      UInt64* xxprime = stackalloc UInt64[5];
      UInt64* zzprime = stackalloc UInt64[5];
      UInt64* zzzprime = stackalloc UInt64[5];

      arrcpy(origx, x, 5);
      fsum(x, z);
      fdifference_backwards(z, origx);  // does x - z

      arrcpy(origxprime, xprime, 5);
      fsum(xprime, zprime);
      fdifference_backwards(zprime, origxprime);
      fmul(xxprime, xprime, z);
      fmul(zzprime, x, zprime);
      arrcpy(origxprime, xxprime, 5);
      fsum(xxprime, zzprime);
      fdifference_backwards(zzprime, origxprime);
      fsquare_times(x3, xxprime, 1);
      fsquare_times(zzzprime, zzprime, 1);
      fmul(z3, zzzprime, qmqp);

      fsquare_times(xx, x, 1);
      fsquare_times(zz, z, 1);
      fmul(x2, xx, zz);
      fdifference_backwards(zz, xx);  // does zz = xx - zz
      fscalar_product(zzz, zz, 121665);
      fsum(zzz, xx);
      fmul(z2, zz, zzz);
    }

    static unsafe void swap_conditional(UInt64* a, UInt64* b, UInt64 iswap)
    {
      uint i;
      UInt64 swap;

      unchecked
      {
        swap = (UInt64)(-((Int64)iswap));
      }

      for (i = 0; i < 5; ++i)
      {
        UInt64 x = swap & (a[i] ^ b[i]);
        a[i] ^= x;
        b[i] ^= x;
      }
    }

    static unsafe void cmult(UInt64* resultx, UInt64* resultz, byte* n, UInt64* q)
    {
      UInt64* a = stackalloc UInt64[5];
      UInt64* b = stackalloc UInt64[5];
      UInt64* c = stackalloc UInt64[5];
      UInt64* d = stackalloc UInt64[5];

      b[0] = 1;
      c[0] = 1;

      UInt64* nqpqx = a;
      UInt64* nqpqz = b;
      UInt64* nqx = c;
      UInt64* nqz = d;
      UInt64* t;

      UInt64* e = stackalloc UInt64[5];
      UInt64* f = stackalloc UInt64[5];
      UInt64* g = stackalloc UInt64[5];
      UInt64* h = stackalloc UInt64[5];
      f[0] = 1;
      h[0] = 1;

      UInt64* nqpqx2 = e;
      UInt64* nqpqz2 = f;
      UInt64* nqx2 = g;
      UInt64* nqz2 = h;

      arrcpy(nqpqx, q, 5);

      for (int i = 0; i < 32; ++i)
      {
        byte byt = n[31 - i];
        for (int j = 0; j < 8; ++j)
        {
          UInt64 bit = (UInt64)(byt >> 7);

          swap_conditional(nqx, nqpqx, bit);
          swap_conditional(nqz, nqpqz, bit);
          fmonty(nqx2, nqz2,
                 nqpqx2, nqpqz2,
                 nqx, nqz,
                 nqpqx, nqpqz,
                 q);
          swap_conditional(nqx2, nqpqx2, bit);
          swap_conditional(nqz2, nqpqz2, bit);

          t = nqx;
          nqx = nqx2;
          nqx2 = t;
          t = nqz;
          nqz = nqz2;
          nqz2 = t;
          t = nqpqx;
          nqpqx = nqpqx2;
          nqpqx2 = t;
          t = nqpqz;
          nqpqz = nqpqz2;
          nqpqz2 = t;

          byt <<= 1;
        }
      }

      arrcpy(resultx, nqx, 5);
      arrcpy(resultz, nqz, 5);
    }


    // -----------------------------------------------------------------------------
    // Shamelessly copied from djb's code, tightened a little
    // -----------------------------------------------------------------------------
    unsafe static void crecip(UInt64* output, UInt64* z)
    {
      UInt64* a = stackalloc UInt64[5];
      UInt64* t0 = stackalloc UInt64[5];
      UInt64* b = stackalloc UInt64[5];
      UInt64* c = stackalloc UInt64[5];

      /* 2 */
      fsquare_times(a, z, 1); // a = 2
      /* 8 */
      fsquare_times(t0, a, 2);
      /* 9 */
      fmul(b, t0, z); // b = 9
      /* 11 */
      fmul(a, b, a); // a = 11
      /* 22 */
      fsquare_times(t0, a, 1);
      /* 2^5 - 2^0 = 31 */
      fmul(b, t0, b);
      /* 2^10 - 2^5 */
      fsquare_times(t0, b, 5);
      /* 2^10 - 2^0 */
      fmul(b, t0, b);
      /* 2^20 - 2^10 */
      fsquare_times(t0, b, 10);
      /* 2^20 - 2^0 */
      fmul(c, t0, b);
      /* 2^40 - 2^20 */
      fsquare_times(t0, c, 20);
      /* 2^40 - 2^0 */
      fmul(t0, t0, c);
      /* 2^50 - 2^10 */
      fsquare_times(t0, t0, 10);
      /* 2^50 - 2^0 */
      fmul(b, t0, b);
      /* 2^100 - 2^50 */
      fsquare_times(t0, b, 50);
      /* 2^100 - 2^0 */
      fmul(c, t0, b);
      /* 2^200 - 2^100 */
      fsquare_times(t0, c, 100);
      /* 2^200 - 2^0 */
      fmul(t0, t0, c);
      /* 2^250 - 2^50 */
      fsquare_times(t0, t0, 50);
      /* 2^250 - 2^0 */
      fmul(t0, t0, b);
      /* 2^255 - 2^5 */
      fsquare_times(t0, t0, 5);
      /* 2^255 - 21 */
      fmul(output, t0, a);
    }


  }
}
