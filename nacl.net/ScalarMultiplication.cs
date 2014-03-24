using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nacl
{
  public static class ScalarMultiplication
  {
    private static readonly bool m_is64Bit = IntPtr.Size == 8;

    public const int OutputSize = 32;

    public const int ScalarSize = 32;

    public static void MultiplyBase(byte[] q, byte[] n)
    {
      if (m_is64Bit)
      {
        ScalarMultiplication64.MultiplyBase(q, n);
      }
      else
      {
        ScalarMultiplication32.MultiplyBase(q, n);
      }
    }

    public static void Multiply(byte[] q, byte[] n, byte[] p)
    {
      if (m_is64Bit)
      {
        ScalarMultiplication64.Multiply(q, n, p);
      }
      else
      {
        ScalarMultiplication32.Multiply(q, n, p);
      }
    }
  }
}
