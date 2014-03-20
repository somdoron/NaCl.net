using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nacl;
using NUnit.Framework;

namespace Nacl.net.Tests
{
  [TestFixture]
  public class SecretAuthenticationTests
  {
    [Test]
    public void Test1HMACSHA512256()
    {
      byte[] key = Encoding.ASCII.GetBytes("Jefe");

      byte[] c = Encoding.ASCII.GetBytes("what do ya want for nothing?");

      byte[] a = new byte[32];


      int i;

      SecretAuthentication secretAuthentication = new SecretAuthentication(SecretAuthenticationPrimitive.HMACSHA512256);

      secretAuthentication.Authenticate(a, c, key);

      for (i = 0; i < 32; ++i)
      {
        Console.Write(",0x{0:x}", a[i]);
        if (i % 8 == 7) Console.WriteLine();
      }

      Assert.IsTrue(a.SequenceEqual(new byte[]{0x16,0x4b,0x7a,0x7b,0xfc,0xf8,0x19,0xe2
        ,0xe3,0x95,0xfb,0xe7,0x3b,0x56,0xe0,0xa3
        ,0x87,0xbd,0x64,0x22,0x2e,0x83,0x1f,0xd6
        ,0x10,0x27,0x0c,0xd7,0xea,0x25,0x05,0x54}));
    }
  }
}
