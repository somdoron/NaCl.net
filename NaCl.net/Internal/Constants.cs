using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaCl.Internal
{
  internal static class Constants
  {
    public static readonly byte[] Sigma = new byte[] { (byte)'e', (byte)'x', (byte)'p', 
            (byte)'a', (byte)'n', (byte)'d', (byte)' ', (byte)'3', (byte)'2', (byte)'-', 
            (byte)'b', (byte)'y', (byte)'t', (byte)'e', (byte)' ', (byte)'k' };

    public static readonly byte[] N = new byte[32];    

    public static readonly uint[] MinUsp = { 19, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 128 };   
  }
}
