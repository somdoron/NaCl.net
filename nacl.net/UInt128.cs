using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace nacl
{
  public struct UInt128
  {
    private UInt64 m_high;
    private UInt64 m_low;

    public UInt128(UInt64 n)
    {
      m_low = n;
      m_high = 0;
    }

    public UInt128(UInt64 high, UInt64 low)
    {
      m_low = low;
      m_high = high;
    }

    public static explicit operator UInt64(UInt128 value)
    {
      return value.m_low;
    }

    public static explicit operator UInt128(UInt64 value)
    {
      return new UInt128(value);
    }   

    public static UInt128 MultiplyUInt64(UInt64 left, UInt64 right)
    {      
      UInt64 a32 = left >> 32;
      UInt64 a00 = left & 0xffffffffu;

      UInt64 b32 = right >> 32;
      UInt64 b00 = right & 0xffffffffu;           

      ulong high = a32 * b32;

      ulong low = a00 * b00;

      ulong addLow = (a32 * b00 + a00 * b32);
      ulong addHigh = addLow >> 32;
      addLow = addLow << 32;

      ulong c = (((low & addLow) & 1) + (low >> 1) + (addLow >> 1)) >> 63;
      high += addHigh + c;
      low +=  addLow;

      return new UInt128(high, low);
    }

    public void Add(UInt128 n)
    {
      ulong c = (((m_low & n.m_low) & 1) + (m_low >> 1) + (n.m_low >> 1)) >> 63;
      m_high = m_high + n.m_high + c;
      m_low = m_low + n.m_low;      
    }

    public void Add(UInt64 n)
    {
      ulong c = (((m_low & n) & 1) + (m_low >> 1) + (n >> 1)) >> 63;
      m_high = m_high + c;
      m_low = m_low + n;      
    }

    public static UInt128 operator *(UInt128 left, UInt64 right)
    {
      unchecked
      {
        UInt64 a96 = left.m_high >> 32;
        UInt64 a64 = left.m_high & 0xffffffffu;
        UInt64 a32 = left.m_low >> 32;
        UInt64 a00 = left.m_low & 0xffffffffu;

        UInt64 b32 = right >> 32;
        UInt64 b00 = right & 0xffffffffu;

        // multiply [a96 .. a00] x [b96 .. b00]
        // terms higher than c96 disappear off the high side
        // terms c96 and c64 are safe to ignore carry bit
        UInt64 c96 = a96 * b00 + a64 * b32;
        UInt64 c64 = a64 * b00 + a32 * b32;

        ulong high = (c96 << 32) + c64;
        ulong low = a00 * b00;        

        ulong addLow = (a32 * b00 + a00 * b32);
        ulong addHigh = addLow >> 32;        
        addLow = addLow << 32;

        ulong c = (((low & addLow) & 1) + (low >> 1) + (addLow >> 1)) >> 63;
        high = high + addHigh + c;
        low = low + addLow;

        return new UInt128(high, low);
      }
    }

    public static UInt128 operator +(UInt128 left, UInt64 right)
    {
      ulong c = (((left.m_low & right) & 1) + (left.m_low >> 1) + (right >> 1)) >> 63;
      ulong high = left.m_high + c;
      ulong low = left.m_low + right;

      return new UInt128(high, low);
    }

    public static UInt128 operator +(UInt128 left, UInt128 right)
    {  
      ulong c = (((left.m_low & right.m_low) & 1) + (left.m_low >> 1) + (right.m_low >> 1)) >> 63;
      ulong high = left.m_high + right.m_high + c;
      ulong low = left.m_low + right.m_low;    

      return new UInt128(high, low);
    }

    public static UInt128 operator >>(UInt128 value, int shift)
    {
      UInt128 shifted = new UInt128();

      if (shift > 63)
      {
        shifted.m_low = value.m_high >> (shift - 64);
        shifted.m_high = 0;
      }
      else
      {
        shifted.m_high = value.m_high >> shift;
        shifted.m_low = (value.m_high << (64 - shift)) | (value.m_low >> shift);
      }
      return shifted;
    }

    public static UInt128 operator <<(UInt128 value, int shift)
    {
      UInt128 shifted = new UInt128();

      if (shift > 63)
      {
        shifted.m_high = value.m_low << (shift - 64);
        shifted.m_low = 0;
      }
      else
      {
        ulong ul = value.m_low >> (64 - shift);
        shifted.m_high = ul | (value.m_high << shift);
        shifted.m_low = value.m_low << shift;
      }
      return shifted;
    }

    public static UInt128 operator &(UInt128 left, UInt64 right)
    {
      UInt128 result = left;
      result.m_high = 0;
      result.m_low &= right;

      return result;
    }

    public static UInt128 operator &(UInt128 left, UInt128 right)
    {
      UInt128 result = left;
      result.m_high &= right.m_high;
      result.m_low &= right.m_low;

      return result;
    }

    public static UInt128 operator |(UInt128 left, UInt128 right)
    {
      UInt128 result = left;
      result.m_high |= right.m_high;
      result.m_low |= right.m_low;
      return result;
    }
  }
}
