using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nacl
{
  public static class SafeComparison
  {
    public static bool Verify16(byte[] x, byte[] y)
    {
      return Verify16(x,0, y,0);
    }

    public static bool Verify16(byte[] x, int xOffset, byte[] y, int yOffset)
    {  
      uint differentBits = 0;

      differentBits |= (uint)((x[0 + xOffset] ^ y[0 + yOffset]));
      differentBits |= (uint)((x[1 + xOffset] ^ y[1 + yOffset]));
      differentBits |= (uint)((x[2 + xOffset] ^ y[2 + yOffset]));
      differentBits |= (uint)((x[3 + xOffset] ^ y[3 + yOffset]));
      differentBits |= (uint)((x[4 + xOffset] ^ y[4 + yOffset]));
      differentBits |= (uint)((x[5 + xOffset] ^ y[5 + yOffset]));
      differentBits |= (uint)((x[6 + xOffset] ^ y[6 + yOffset]));
      differentBits |= (uint)((x[7 + xOffset] ^ y[7 + yOffset]));
      differentBits |= (uint)((x[8 + xOffset] ^ y[8 + yOffset]));
      differentBits |= (uint)((x[9 + xOffset] ^ y[9 + yOffset]));
      differentBits |= (uint)((x[10 + xOffset] ^ y[10 + yOffset]));
      differentBits |= (uint)((x[11 + xOffset] ^ y[11 + yOffset]));
      differentBits |= (uint)((x[12 + xOffset] ^ y[12 + yOffset]));
      differentBits |= (uint)((x[13 + xOffset] ^ y[13 + yOffset]));
      differentBits |= (uint)((x[14 + xOffset] ^ y[14 + yOffset]));
      differentBits |= (uint)((x[15 + xOffset] ^ y[15 + yOffset]));

      return (1 & ((differentBits - 1) >> 8)) == 1;
    }

    public static bool Verify32(byte[] x, byte[] y)
    {
      return Verify32(x, 0,y,0);
    }

    public static bool Verify32(byte[] x, int xOffset, byte[] y, int yOffset)
    {   
      uint differentBits = 0;

      differentBits |= (uint)((x[xOffset +0] ^ y[yOffset +0]));
      differentBits |= (uint)((x[xOffset +1] ^ y[yOffset +1]));
      differentBits |= (uint)((x[xOffset +2] ^ y[yOffset +2]));
      differentBits |= (uint)((x[xOffset +3] ^ y[yOffset +3]));
      differentBits |= (uint)((x[xOffset +4] ^ y[yOffset +4]));
      differentBits |= (uint)((x[xOffset +5] ^ y[yOffset +5]));
      differentBits |= (uint)((x[xOffset +6] ^ y[yOffset +6]));
      differentBits |= (uint)((x[xOffset +7] ^ y[yOffset +7]));
      differentBits |= (uint)((x[xOffset +8] ^ y[yOffset +8]));
      differentBits |= (uint)((x[xOffset +9] ^ y[yOffset +9]));
      differentBits |= (uint)((x[xOffset +10] ^ y[yOffset +10]));
      differentBits |= (uint)((x[xOffset +11] ^ y[yOffset +11]));
      differentBits |= (uint)((x[xOffset +12] ^ y[yOffset +12]));
      differentBits |= (uint)((x[xOffset +13] ^ y[yOffset +13]));
      differentBits |= (uint)((x[xOffset +14] ^ y[yOffset +14]));
      differentBits |= (uint)((x[xOffset +15] ^ y[yOffset +15]));
      differentBits |= (uint)((x[xOffset +16] ^ y[yOffset +16]));
      differentBits |= (uint)((x[xOffset +17] ^ y[yOffset +17]));
      differentBits |= (uint)((x[xOffset +18] ^ y[yOffset +18]));
      differentBits |= (uint)((x[xOffset +19] ^ y[yOffset +19]));
      differentBits |= (uint)((x[xOffset +20] ^ y[yOffset +20]));
      differentBits |= (uint)((x[xOffset +21] ^ y[yOffset +21]));
      differentBits |= (uint)((x[xOffset +22] ^ y[yOffset +22]));
      differentBits |= (uint)((x[xOffset +23] ^ y[yOffset +23]));
      differentBits |= (uint)((x[xOffset +24] ^ y[yOffset +24]));
      differentBits |= (uint)((x[xOffset +25] ^ y[yOffset +25]));
      differentBits |= (uint)((x[xOffset +26] ^ y[yOffset +26]));
      differentBits |= (uint)((x[xOffset +27] ^ y[yOffset +27]));
      differentBits |= (uint)((x[xOffset +28] ^ y[yOffset +28]));
      differentBits |= (uint)((x[xOffset +29] ^ y[yOffset +29]));
      differentBits |= (uint)((x[xOffset +30] ^ y[yOffset +30]));
      differentBits |= (uint)((x[xOffset +31] ^ y[yOffset +31]));      

      return (1 & ((differentBits - 1) >> 8)) == 1;
    }
  }
}
