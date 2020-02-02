using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaCl.Internal
{
    internal static class SafeComparison
    {
        public static bool Verify16(ReadOnlySpan<byte> x, ReadOnlySpan<byte> y)
        {
            uint differentBits = 0;

            differentBits |= (uint) ((x[0] ^ y[0]));
            differentBits |= (uint) ((x[1] ^ y[1]));
            differentBits |= (uint) ((x[2] ^ y[2]));
            differentBits |= (uint) ((x[3] ^ y[3]));
            differentBits |= (uint) ((x[4] ^ y[4]));
            differentBits |= (uint) ((x[5] ^ y[5]));
            differentBits |= (uint) ((x[6] ^ y[6]));
            differentBits |= (uint) ((x[7] ^ y[7]));
            differentBits |= (uint) ((x[8] ^ y[8]));
            differentBits |= (uint) ((x[9] ^ y[9]));
            differentBits |= (uint) ((x[10] ^ y[10]));
            differentBits |= (uint) ((x[11] ^ y[11]));
            differentBits |= (uint) ((x[12] ^ y[12]));
            differentBits |= (uint) ((x[13] ^ y[13]));
            differentBits |= (uint) ((x[14] ^ y[14]));
            differentBits |= (uint) ((x[15] ^ y[15]));

            return (1 & ((differentBits - 1) >> 8)) == 1;
        }

        public static bool Verify32(Span<byte> x, Span<byte> y)
        {
            uint differentBits = 0;

            differentBits |= (uint) ((x[0] ^ y[0]));
            differentBits |= (uint) ((x[1] ^ y[1]));
            differentBits |= (uint) ((x[2] ^ y[2]));
            differentBits |= (uint) ((x[3] ^ y[3]));
            differentBits |= (uint) ((x[4] ^ y[4]));
            differentBits |= (uint) ((x[5] ^ y[5]));
            differentBits |= (uint) ((x[6] ^ y[6]));
            differentBits |= (uint) ((x[7] ^ y[7]));
            differentBits |= (uint) ((x[8] ^ y[8]));
            differentBits |= (uint) ((x[9] ^ y[9]));
            differentBits |= (uint) ((x[10] ^ y[10]));
            differentBits |= (uint) ((x[11] ^ y[11]));
            differentBits |= (uint) ((x[12] ^ y[12]));
            differentBits |= (uint) ((x[13] ^ y[13]));
            differentBits |= (uint) ((x[14] ^ y[14]));
            differentBits |= (uint) ((x[15] ^ y[15]));
            differentBits |= (uint) ((x[16] ^ y[16]));
            differentBits |= (uint) ((x[17] ^ y[17]));
            differentBits |= (uint) ((x[18] ^ y[18]));
            differentBits |= (uint) ((x[19] ^ y[19]));
            differentBits |= (uint) ((x[20] ^ y[20]));
            differentBits |= (uint) ((x[21] ^ y[21]));
            differentBits |= (uint) ((x[22] ^ y[22]));
            differentBits |= (uint) ((x[23] ^ y[23]));
            differentBits |= (uint) ((x[24] ^ y[24]));
            differentBits |= (uint) ((x[25] ^ y[25]));
            differentBits |= (uint) ((x[26] ^ y[26]));
            differentBits |= (uint) ((x[27] ^ y[27]));
            differentBits |= (uint) ((x[28] ^ y[28]));
            differentBits |= (uint) ((x[29] ^ y[29]));
            differentBits |= (uint) ((x[30] ^ y[30]));
            differentBits |= (uint) ((x[31] ^ y[31]));

            return (1 & ((differentBits - 1) >> 8)) == 1;
        }
    }
}