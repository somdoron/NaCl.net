using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace nacl
{
  public struct UInt128
  {    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public UInt128(UInt64 n) 
    {
      Low = n;
      High = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public UInt128(UInt64 high, UInt64 low) 
    {
      Low = low;
      High = high;
    }

    public ulong High;
    public ulong Low;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator UInt64(UInt128 value)
    {
      return value.Low;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator UInt128(UInt64 value)
    {
      return new UInt128(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetMultiplyUInt64(ref UInt64 left, ref UInt64 right)
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
      low += addLow;

      High = high;
      Low = low;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddMultiplyUInt64(ref UInt64 left,ref UInt64 right)
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
      low += addLow;

      c = (((Low & low) & 1) + (Low >> 1) + (low >> 1)) >> 63;
      High = High + high + c;
      Low = Low + low;      
    }
    

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(UInt128 n)
    {
      ulong c = (((Low & n.Low) & 1) + (Low >> 1) + (n.Low >> 1)) >> 63;
      High = High + n.High + c;
      Low = Low + n.Low;      
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(UInt64 n)
    {
      ulong c = (((Low & n) & 1) + (Low >> 1) + (n >> 1)) >> 63;
      High = High + c;
      Low = Low + n;      
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt128 operator *(UInt128 left, UInt64 right)
    {
      unchecked
      {
        UInt64 a96 = left.High >> 32;
        UInt64 a64 = left.High & 0xffffffffu;
        UInt64 a32 = left.Low >> 32;
        UInt64 a00 = left.Low & 0xffffffffu;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt128 operator +(UInt128 left, UInt64 right)
    {
      ulong c = (((left.Low & right) & 1) + (left.Low >> 1) + (right >> 1)) >> 63;
      ulong high = left.High + c;
      ulong low = left.Low + right;

      return new UInt128(high, low);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt128 operator +(UInt128 left, UInt128 right)
    {  
      ulong c = (((left.Low & right.Low) & 1) + (left.Low >> 1) + (right.Low >> 1)) >> 63;
      ulong high = left.High + right.High + c;
      ulong low = left.Low + right.Low;    

      return new UInt128(high, low);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt128 operator >>(UInt128 value, int shift)
    {
      UInt128 shifted = new UInt128();

      if (shift > 63)
      {
        shifted.Low = value.High >> (shift - 64);
        shifted.High = 0;
      }
      else
      {
        shifted.High = value.High >> shift;
        shifted.Low = (value.High << (64 - shift)) | (value.Low >> shift);
      }
      return shifted;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt128 operator <<(UInt128 value, int shift)
    {
      UInt128 shifted = new UInt128();

      if (shift > 63)
      {
        shifted.High = value.Low << (shift - 64);
        shifted.Low = 0;
      }
      else
      {
        ulong ul = value.Low >> (64 - shift);
        shifted.High = ul | (value.High << shift);
        shifted.Low = value.Low << shift;
      }
      return shifted;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt128 operator &(UInt128 left, UInt64 right)
    {
      UInt128 result = left;
      result.High = 0;
      result.Low &= right;

      return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt128 operator &(UInt128 left, UInt128 right)
    {
      UInt128 result = left;
      result.High &= right.High;
      result.Low &= right.Low;

      return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt128 operator |(UInt128 left, UInt128 right)
    {
      UInt128 result = left;
      result.High |= right.High;
      result.Low |= right.Low;
      return result;
    }
  }
}
