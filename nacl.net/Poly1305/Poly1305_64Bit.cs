using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace nacl.Poly1305
{
  class Poly1305_64Bit : Poly1305
  {
    UInt64[] m_r = new UInt64[3];
    UInt64[] m_h = new UInt64[3];
    UInt64[] m_pad = new UInt64[2];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private UInt64 BytesToInt64(byte[] p, int offset)
    {
      unchecked
      {
        return (
          ((UInt64) (p[0 + offset] & 0xff)) |
          ((UInt64) (p[1 + offset] & 0xff) << 8) |
          ((UInt64) (p[2 + offset] & 0xff) << 16) |
          ((UInt64) (p[3 + offset] & 0xff) << 24) |
          ((UInt64) (p[4 + offset] & 0xff) << 32) |
          ((UInt64) (p[5 + offset] & 0xff) << 40) |
          ((UInt64) (p[6 + offset] & 0xff) << 48) |
          ((UInt64) (p[7 + offset] & 0xff) << 56));
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Int64ToBytes(byte[] p,int offset, UInt64 v)
    {
      unchecked
      {
        p[0 + offset] = (byte) ((v) & 0xff);
        p[1 + offset] = (byte) ((v >> 8) & 0xff);
        p[2 + offset] = (byte) ((v >> 16) & 0xff);
        p[3 + offset] = (byte) ((v >> 24) & 0xff);
        p[4 + offset] = (byte) ((v >> 32) & 0xff);
        p[5 + offset] = (byte) ((v >> 40) & 0xff);
        p[6 + offset] = (byte) ((v >> 48) & 0xff);
        p[7 + offset] = (byte) ((v >> 56) & 0xff);
      }
    }

    protected override void OnKeyChanged()
    {
      unchecked
      {
        UInt64 t0;
        UInt64 t1;

        /* r &= 0xffffffc0ffffffc0ffffffc0fffffff */
        t0 = BytesToInt64(Key, 0);
        t1 = BytesToInt64(Key, 8);

        m_r[0] = (t0) & 0xffc0fffffff;
        m_r[1] = ((t0 >> 44) | (t1 << 20)) & 0xfffffc0ffff;
        m_r[2] = ((t1 >> 24)) & 0x00ffffffc0f;

        m_pad[0] = BytesToInt64(Key, 16);
        m_pad[1] = BytesToInt64(Key, 24);
      }
    }

    protected override void OnReset()
    {
      m_h[0] = 0;
      m_h[1] = 0;
      m_h[2] = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected override void Blocks(byte[] m,int offset, int count)
    {
      unchecked
      {
        UInt64 hibit = m_final ? 0 : ((UInt64) 1 << 40); /* 1 << 128 */
        UInt64 r0, r1, r2;
        UInt64 s1, s2;
        UInt64 h0, h1, h2;
        UInt64 c;
        UInt128 d0 = new UInt128();
        UInt128 d1 = new UInt128();
        UInt128 d2 = new UInt128();

        r0 = m_r[0];
        r1 = m_r[1];
        r2 = m_r[2];

        h0 = m_h[0];
        h1 = m_h[1];
        h2 = m_h[2];

        s1 = r1*(5 << 2);
        s2 = r2*(5 << 2);

        while (count >= BlockSize)
        {
          UInt64 t0, t1;

          /* h += m[i] */
          t0 = BytesToInt64(m, offset);
          t1 = BytesToInt64(m, 8 + offset);

          h0 += ((t0) & 0xfffffffffff);
          h1 += (((t0 >> 44) | (t1 << 20)) & 0xfffffffffff);
          h2 += (((t1 >> 24)) & 0x3ffffffffff) | hibit;

          /* h *= r */
          d0.SetMultiplyUInt64(ref h0, ref  r0);
          d0.AddMultiplyUInt64(ref h1, ref  s2);
          d0.AddMultiplyUInt64(ref h2, ref s1);

          d1.SetMultiplyUInt64(ref h0, ref r1);
          d1.AddMultiplyUInt64(ref h1, ref r0);
          d1.AddMultiplyUInt64(ref h2, ref  s2);

          d2.SetMultiplyUInt64(ref h0, ref r2);
          d2.AddMultiplyUInt64(ref h1, ref  r1);
          d2.AddMultiplyUInt64(ref h2, ref r0);

          /* (partial) h %= p */
          c = (d0 >> 44).Low;
          h0 = d0.Low & 0xfffffffffff;

          d1.Add(c);
          c = (d1 >> 44).Low;
          h1 = d1.Low & 0xfffffffffff;

          d2.Add(c);
          c = (d2 >> 42).Low;
          h2 = d2.Low & 0x3ffffffffff;

          h0 += c*5;
          c = (h0 >> 44);
          h0 = h0 & 0xfffffffffff;
          h1 += c;

          offset += BlockSize;
          count -= BlockSize;
        }

        m_h[0] = h0;
        m_h[1] = h1;
        m_h[2] = h2;
      }
    }

    protected override void OnFinish(byte[] mac, int offset)
    {
      unchecked
      {
        UInt64 h0, h1, h2, c;
        UInt64 g0, g1, g2;
        UInt64 t0, t1;

        /* process the remaining block */
        if (m_leftover != 0)
        {
          int i = m_leftover;
          m_buffer[i] = 1;
          for (i = i + 1; i < BlockSize; i++)
            m_buffer[i] = 0;
          m_final = true;
          Blocks(m_buffer, 0, BlockSize);
        }

        /* fully carry h */
        h0 = m_h[0];
        h1 = m_h[1];
        h2 = m_h[2];

        c = (h1 >> 44);
        h1 &= 0xfffffffffff;
        h2 += c;
        c = (h2 >> 42);
        h2 &= 0x3ffffffffff;
        h0 += c*5;
        c = (h0 >> 44);
        h0 &= 0xfffffffffff;
        h1 += c;
        c = (h1 >> 44);
        h1 &= 0xfffffffffff;
        h2 += c;
        c = (h2 >> 42);
        h2 &= 0x3ffffffffff;
        h0 += c*5;
        c = (h0 >> 44);
        h0 &= 0xfffffffffff;
        h1 += c;

        /* compute h + -p */
        g0 = h0 + 5;
        c = (g0 >> 44);
        g0 &= 0xfffffffffff;
        g1 = h1 + c;
        c = (g1 >> 44);
        g1 &= 0xfffffffffff;
        g2 = h2 + c - ((UInt64) 1 << 42);

        /* select h if h < p, or h + -p if h >= p */
        c = (g2 >> ((sizeof (UInt64)*8) - 1)) - 1;
        g0 &= c;
        g1 &= c;
        g2 &= c;
        c = ~c;
        h0 = (h0 & c) | g0;
        h1 = (h1 & c) | g1;
        h2 = (h2 & c) | g2;

        /* h = (h + pad) */
        t0 = m_pad[0];
        t1 = m_pad[1];

        h0 += ((t0) & 0xfffffffffff);
        c = (h0 >> 44);
        h0 &= 0xfffffffffff;
        h1 += (((t0 >> 44) | (t1 << 20)) & 0xfffffffffff) + c;
        c = (h1 >> 44);
        h1 &= 0xfffffffffff;
        h2 += (((t1 >> 24)) & 0x3ffffffffff) + c;
        h2 &= 0x3ffffffffff;

        /* mac = h % (2^128) */
        h0 = ((h0) | (h1 << 44));
        h1 = ((h1 >> 20) | (h2 << 24));

        Int64ToBytes(mac, offset, h0);
        Int64ToBytes(mac, offset + 8, h1);
      }
    }
  }
}
