using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace nacl.Poly1305
{
  class Poly1305_32Bit : Poly1305
  {
    uint[] m_r = new uint[5];
    uint[] m_h = new uint[5];
    uint[] m_pad = new uint[4];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    uint BytesToInt(byte[] p, int offset)
    {
      unchecked
      {
        return
          (((uint) (p[0 + offset] & 0xff)) |
           ((uint) (p[1 + offset] & 0xff) << 8) |
           ((uint) (p[2 + offset] & 0xff) << 16) |
           ((uint) (p[3 + offset] & 0xff) << 24));
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IntToBytes(byte[] p,int offset, uint v)
    {
      unchecked
      {
        p[0 + offset] = (byte) ((v) & 0xff);
        p[1 + offset] = (byte) ((v >> 8) & 0xff);
        p[2 + offset] = (byte) ((v >> 16) & 0xff);
        p[3 + offset] = (byte) ((v >> 24) & 0xff);
      }
    }

    protected override void OnKeyChanged()
    {
      unchecked
      {
        // r &= 0xffffffc0ffffffc0ffffffc0fffffff 
        m_r[0] = (BytesToInt(Key, KeyOffset + 0 )) & 0x3ffffff;
        m_r[1] = (BytesToInt(Key, KeyOffset + 3) >> 2) & 0x3ffff03;
        m_r[2] = (BytesToInt(Key, KeyOffset + 6) >> 4) & 0x3ffc0ff;
        m_r[3] = (BytesToInt(Key, KeyOffset + 9) >> 6) & 0x3f03fff;
        m_r[4] = (BytesToInt(Key, KeyOffset + 12) >> 8) & 0x00fffff;

        // save pad for later
        m_pad[0] = BytesToInt(Key, KeyOffset + 16);
        m_pad[1] = BytesToInt(Key, KeyOffset + 20);
        m_pad[2] = BytesToInt(Key, KeyOffset + 24);
        m_pad[3] = BytesToInt(Key, KeyOffset + 28);
      }
    }

    protected override void OnReset()
    {
      // h = 0 
      m_h[0] = 0;
      m_h[1] = 0;
      m_h[2] = 0;
      m_h[3] = 0;
      m_h[4] = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected override void Blocks(byte[] m,int offset, int count)
    {
      unchecked
      {
        uint hibit = (uint) ((m_final) ? 0 : (1 << 24)); /* 1 << 128 */
        UInt32 r0, r1, r2, r3, r4;
        UInt32 s1, s2, s3, s4;
        UInt32 h0, h1, h2, h3, h4;
        UInt64 d0, d1, d2, d3, d4;
        UInt32 c;

        r0 = m_r[0];
        r1 = m_r[1];
        r2 = m_r[2];
        r3 = m_r[3];
        r4 = m_r[4];

        s1 = r1*5;
        s2 = r2*5;
        s3 = r3*5;
        s4 = r4*5;

        h0 = m_h[0];
        h1 = m_h[1];
        h2 = m_h[2];
        h3 = m_h[3];
        h4 = m_h[4];

        while (count >= BlockSize)
        {
          /* h += m[i] */
          h0 += (BytesToInt(m, offset)) & 0x3ffffff;
          h1 += (BytesToInt(m, 3 + offset) >> 2) & 0x3ffffff;
          h2 += (BytesToInt(m, 6 + offset) >> 4) & 0x3ffffff;
          h3 += (BytesToInt(m, 9 + offset) >> 6) & 0x3ffffff;
          h4 += (BytesToInt(m, 12 + offset) >> 8) | hibit;

          /* h *= r */
          d0 = ((UInt64) h0*r0) + ((UInt64) h1*s4) + ((UInt64) h2*s3) + ((UInt64) h3*s2) + ((UInt64) h4*s1);
          d1 = ((UInt64) h0*r1) + ((UInt64) h1*r0) + ((UInt64) h2*s4) + ((UInt64) h3*s3) + ((UInt64) h4*s2);
          d2 = ((UInt64) h0*r2) + ((UInt64) h1*r1) + ((UInt64) h2*r0) + ((UInt64) h3*s4) + ((UInt64) h4*s3);
          d3 = ((UInt64) h0*r3) + ((UInt64) h1*r2) + ((UInt64) h2*r1) + ((UInt64) h3*r0) + ((UInt64) h4*s4);
          d4 = ((UInt64) h0*r4) + ((UInt64) h1*r3) + ((UInt64) h2*r2) + ((UInt64) h3*r1) + ((UInt64) h4*r0);

          /* (partial) h %= p */
          c = (UInt32) (d0 >> 26);
          h0 = (UInt32) d0 & 0x3ffffff;
          d1 += c;
          c = (UInt32) (d1 >> 26);
          h1 = (UInt32) d1 & 0x3ffffff;
          d2 += c;
          c = (UInt32) (d2 >> 26);
          h2 = (UInt32) d2 & 0x3ffffff;
          d3 += c;
          c = (UInt32) (d3 >> 26);
          h3 = (UInt32) d3 & 0x3ffffff;
          d4 += c;
          c = (UInt32) (d4 >> 26);
          h4 = (UInt32) d4 & 0x3ffffff;
          h0 += c*5;
          c = (h0 >> 26);
          h0 = h0 & 0x3ffffff;
          h1 += c;

          offset += BlockSize;
          count -= BlockSize;
        }

        m_h[0] = h0;
        m_h[1] = h1;
        m_h[2] = h2;
        m_h[3] = h3;
        m_h[4] = h4;
      }
    }

    protected override void OnFinish(byte[] mac, int offset)
    {
      unchecked
      {
        uint h0, h1, h2, h3, h4, c;
        uint g0, g1, g2, g3, g4;
        UInt64 f;
        uint mask;

        /* process the remaining block */
        if (m_leftover != 0)
        {
          int i = m_leftover;
          m_buffer[i++] = 1;
          for (; i < BlockSize; i++)
            m_buffer[i] = 0;
          m_final = true;
          Blocks(m_buffer, 0, BlockSize);
        }

        /* fully carry h */
        h0 = m_h[0];
        h1 = m_h[1];
        h2 = m_h[2];
        h3 = m_h[3];
        h4 = m_h[4];

        c = h1 >> 26;
        h1 = h1 & 0x3ffffff;
        h2 += c;
        c = h2 >> 26;
        h2 = h2 & 0x3ffffff;
        h3 += c;
        c = h3 >> 26;
        h3 = h3 & 0x3ffffff;
        h4 += c;
        c = h4 >> 26;
        h4 = h4 & 0x3ffffff;
        h0 += c*5;
        c = h0 >> 26;
        h0 = h0 & 0x3ffffff;
        h1 += c;

        /* compute h + -p */
        g0 = h0 + 5;
        c = g0 >> 26;
        g0 &= 0x3ffffff;
        g1 = h1 + c;
        c = g1 >> 26;
        g1 &= 0x3ffffff;
        g2 = h2 + c;
        c = g2 >> 26;
        g2 &= 0x3ffffff;
        g3 = h3 + c;
        c = g3 >> 26;
        g3 &= 0x3ffffff;
        g4 = h4 + c - (1 << 26);

        /* select h if h < p, or h + -p if h >= p */
        mask = (g4 >> ((sizeof (uint)*8) - 1)) - 1;
        g0 &= mask;
        g1 &= mask;
        g2 &= mask;
        g3 &= mask;
        g4 &= mask;
        mask = ~mask;
        h0 = (h0 & mask) | g0;
        h1 = (h1 & mask) | g1;
        h2 = (h2 & mask) | g2;
        h3 = (h3 & mask) | g3;
        h4 = (h4 & mask) | g4;

        /* h = h % (2^128) */
        h0 = ((h0) | (h1 << 26)) & 0xffffffff;
        h1 = ((h1 >> 6) | (h2 << 20)) & 0xffffffff;
        h2 = ((h2 >> 12) | (h3 << 14)) & 0xffffffff;
        h3 = ((h3 >> 18) | (h4 << 8)) & 0xffffffff;

        /* mac = (h + pad) % (2^128) */
        f = (UInt64) h0 + m_pad[0];
        h0 = (uint) f;

        f = (UInt64) h1 + m_pad[1] + (f >> 32);
        h1 = (uint) f;

        f = (UInt64) h2 + m_pad[2] + (f >> 32);
        h2 = (uint) f;

        f = (UInt64) h3 + m_pad[3] + (f >> 32);
        h3 = (uint) f;

        IntToBytes(mac, 0 + offset, h0);
        IntToBytes(mac, 4 + offset, h1);
        IntToBytes(mac, 8 + offset, h2);
        IntToBytes(mac, 12 + offset, h3);
      }
    }
  }
}
