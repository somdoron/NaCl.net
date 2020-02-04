using System;
using NaCl.Internal;
using bytes = System.Span<byte>;
using fe25519 = System.Span<int>;

namespace NaCl
{
    /// <summary>
    /// NaCl.net provides an API to multiply a point on the Curve25519 curve.
    /// This can be used as a building block to construct key exchange mechanisms, or more generally to compute a public key from a secret key.
    /// </summary>
    public static class Curve25519
    {
        /// <summary>
        /// Length of a scalar on curve.
        /// </summary>
        /// <remarks>
        /// 32 bytes length.
        /// </remarks>
        public const int ScalarLength = 32;

        /// <summary>
        /// This function can be used to compute a shared secret q given a user's secret key and another user's public key.
        /// </summary>
        /// <param name="q">Resulted shared secret</param>
        /// <param name="n">Secret key of alice</param>
        /// <param name="p">Public key of bob</param>
        public static void ScalarMultiplication(bytes q, ReadOnlySpan<byte> n, ReadOnlySpan<byte> p)
        {
            bytes t = q;

            fe25519 x1 = stackalloc int[Fe25519.ArraySize];
            fe25519 x2 = stackalloc int[Fe25519.ArraySize];
            fe25519 z2 = stackalloc int[Fe25519.ArraySize];
            fe25519 x3 = stackalloc int[Fe25519.ArraySize];
            fe25519 z3 = stackalloc int[Fe25519.ArraySize];
            fe25519 tmp0 = stackalloc int[Fe25519.ArraySize];
            fe25519 tmp1 = stackalloc int[Fe25519.ArraySize];
            int pos;
            int swap;
            int b;

            for (int i = 0; i < 32; i++)
                t[i] = n[i];
            t[0] &= 248;
            t[31] &= 127;
            t[31] |= 64;
            Fe25519.FromBytes(x1, p);
            Fe25519.One(x2);
            Fe25519.Zero(z2);
            Fe25519.Copy(x3, x1);
            Fe25519.One(z3);

            swap = 0;
            for (pos = 254; pos >= 0; --pos)
            {
                b = t[pos / 8] >> (pos & 7);
                b &= 1;
                swap ^= b;
                Fe25519.CSwap(x2, x3, swap);
                Fe25519.CSwap(z2, z3, swap);
                swap = b;
                Fe25519.Sub(tmp0, x3, z3);
                Fe25519.Sub(tmp1, x2, z2);
                Fe25519.Add(x2, x2, z2);
                Fe25519.Add(z2, x3, z3);
                Fe25519.Mul(z3, tmp0, x2);
                Fe25519.Mul(z2, z2, tmp1);
                Fe25519.Sq(tmp0, tmp1);
                Fe25519.Sq(tmp1, x2);
                Fe25519.Add(x3, z3, z2);
                Fe25519.Sub(z2, z3, z2);
                Fe25519.Mul(x2, tmp1, tmp0);
                Fe25519.Sub(tmp1, tmp1, tmp0);
                Fe25519.Sq(z2, z2);
                Fe25519.Mul121666(z3, tmp1);
                Fe25519.Sq(x3, x3);
                Fe25519.Add(tmp0, tmp0, z3);
                Fe25519.Mul(z3, x1, z2);
                Fe25519.Mul(z2, tmp1, tmp0);
            }

            Fe25519.CSwap(x2, x3, swap);
            Fe25519.CSwap(z2, z3, swap);

            Fe25519.Invert(z2, z2);
            Fe25519.Mul(x2, x2, z2);
            Fe25519.ToBytes(q, x2);
        }

        /// <summary>
        /// This function can be used to compute a shared secret q given a user's secret key and another user's public key.
        /// </summary>
        /// <param name="n">Secret key of alice</param>
        /// <param name="p">Public key of bob</param>
        /// <returns>Shared key</returns>
        public static byte[] ScalarMultiplication(ReadOnlySpan<byte> n, ReadOnlySpan<byte> p)
        {
            var q = new byte[ScalarLength];
            ScalarMultiplication(q, n, p);
            return q;
        }

        /// <summary>
        /// This function can be used to compute a shared secret q given a user's secret key and another user's public key.
        /// </summary>
        /// <param name="n">Secret key of alice</param>
        /// <param name="p">Public key of bob</param>
        /// <returns>Shared key</returns>
        public static byte[] ScalarMultiplication(byte[] n, byte[] p)
        {
            return ScalarMultiplication(new bytes(n, 0, ScalarLength), new bytes(p, 0, ScalarLength));
        }

        /// <summary>
        /// This function can be used to compute a shared secret q given a user's secret key and another user's public key.
        /// </summary>
        /// <param name="q">Resulted shared secret</param>
        /// <param name="n">Secret key of alice</param>
        /// <param name="p">Public key of bob</param>
        public static void ScalarMultiplication(byte[] q, byte[] n, byte[] p)
        {
            ScalarMultiplication(new bytes(q, 0, ScalarLength), new bytes(n, 0, ScalarLength), new bytes(p, 0, ScalarLength));
        }

        /// <summary>
        /// This function can be used to compute a shared secret q given a user's secret key and another user's public key.
        /// </summary>
        /// <param name="q">Resulted shared secret</param>
        /// <param name="qOffset">Shared key offset to start write to</param>
        /// <param name="n">Secret key of alice</param>
        /// <param name="nOffset">Secret key offset to start read from</param>
        /// <param name="p">Public key of bob</param>
        /// <param name="pOffset">Public key offset, to start read from</param>
        public static void ScalarMultiplication(byte[] q, int qOffset, byte[] n, int nOffset, byte[] p, int pOffset)
        {
            ScalarMultiplication(new bytes(q, qOffset, ScalarLength), new bytes(n, nOffset, ScalarLength), new bytes(p, pOffset, ScalarLength));
        }

        /// <summary>
        /// Given a user's secret key n (<see cref="ScalarLength"/> length), the ScalarMultiplicationBase function computes the user's public key and puts it into q.
        /// </summary>
        /// <param name="q">Public key, result of the multiplication</param>
        /// <param name="n">Secret key, which will be multiplied with base</param>
        public static void ScalarMultiplicationBase(bytes q, bytes n)
        {
            bytes p = stackalloc byte[ScalarLength];
            p[0] = 9;

            ScalarMultiplication(q, n, p);
        }

        /// <summary>
        /// Given a user's secret key n (<see cref="ScalarLength"/> length), the ScalarMultiplicationBase function computes the user's public key and puts it into q.
        /// </summary>
        /// <param name="q">Public key, result of the multiplication</param>
        /// <param name="n">Secret key, which will be multiplied with base</param>
        public static void ScalarMultiplicationBase(byte[] q, byte[] n)
        {
            ScalarMultiplicationBase(new bytes(q, 0, ScalarLength), new bytes(n, 0, ScalarLength));
        }

        /// <summary>
        /// Given a user's secret key n (<see cref="ScalarLength"/> length), the ScalarMultiplicationBase function computes the user's public key and puts it into q.
        /// </summary>
        /// <param name="q">Public key, result of the multiplication</param>
        /// <param name="qOffset">Public key offset to start write the in</param>
        /// <param name="n">Secret key, which will be multiplied with base</param>
        /// <param name="nOffset">Secret key offset, to start read from</param>
        public static void ScalarMultiplicationBase(byte[] q, int qOffset, byte[] n, int nOffset)
        {
            ScalarMultiplicationBase(new bytes(q, qOffset, ScalarLength), new bytes(n, nOffset, ScalarLength));
        }

        /// <summary>
        /// Given a user's secret key n (<see cref="ScalarLength"/> length), the ScalarMultiplicationBase function computes the user's public key and puts it into q.
        /// </summary>
        /// <param name="n">Secret key, which will be multiplied with base</param>
        /// <returns>Returns q, the public key</returns>
        public static byte[] ScalarMultiplicationBase(bytes n)
        {
            var q = new byte[ScalarLength];
            ScalarMultiplicationBase(q, n);
            return q;
        }

        /// <summary>
        /// Given a user's secret key n (<see cref="ScalarLength"/> length), the ScalarMultiplicationBase function computes the user's public key and puts it into q.
        /// </summary>
        /// <param name="n">Secret key, which will be multiplied with base</param>
        /// <returns>Returns q, the public key</returns>
        public static byte[] ScalarMultiplicationBase(byte[] n)
        {
            var q = new byte[ScalarLength];
            ScalarMultiplicationBase(q, n);
            return q;
        }
    }
}