using System;
using System.Linq;
using NUnit.Framework;
using NaCl;

namespace NaCl.Tests
{
    [TestFixture]
    public class Curve25519Tests
    {
        public static byte[] FromHex(string hex) {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }
        
        [Test]
        public void Curve25519Test1()
        {
            byte[] alicesk = new byte[]
            {
                0x77, 0x07, 0x6d, 0x0a, 0x73, 0x18, 0xa5, 0x7d, 0x3c, 0x16, 0xc1,
                0x72, 0x51, 0xb2, 0x66, 0x45, 0xdf, 0x4c, 0x2f, 0x87, 0xeb, 0xc0,
                0x99, 0x2a, 0xb1, 0x77, 0xfb, 0xa5, 0x1d, 0xb9, 0x2c, 0x2a
            };

            byte[] bobsk = new byte[]
            {
                0x5d, 0xab, 0x08, 0x7e, 0x62, 0x4a, 0x8a, 0x4b, 0x79, 0xe1, 0x7f,
                0x8b, 0x83, 0x80, 0x0e, 0xe6, 0x6f, 0x3b, 0xb1, 0x29, 0x26, 0x18,
                0xb6, 0xfd, 0x1c, 0x2f, 0x8b, 0x27, 0xff, 0x88, 0xe0, 0xeb
            };

            var alicepk = new byte[32];
            var bobpk = new byte[32];
            
            Curve25519.ScalarMultiplicationBase(alicepk, alicesk);
            Assert.AreEqual(
                FromHex("8520f0098930a754748b7ddcb43ef75a0dbf3a0d26381af4eba4a98eaa9b4e6a"),
                alicepk);

            Curve25519.ScalarMultiplicationBase(bobpk, bobsk);
            Assert.AreEqual(
                FromHex("de9edb7d7b7dc1b4d35b61c2ece435373f8343c85b78674dadfc7e146f882b4f"),
                bobpk);

            var key1 = new byte[32];
            Curve25519.ScalarMultiplication(key1, alicesk, bobpk);

            var key2 = new byte[32];
            Curve25519.ScalarMultiplication(key2, bobsk, alicepk);
            
            Assert.AreEqual(key1, key2);
            Assert.AreEqual(
                FromHex("4a5d9d5ba4ce2de1728e3bf480350f25e07e21c947d19e3376f09b3c1e161742"),
                key1
                );
        }
    }
}