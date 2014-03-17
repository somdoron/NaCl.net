using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using nacl.Core;

namespace nacl
{
    public class Box
    {
        public const int PublicKeyBytes = 32;
        public const int SecretKeyBytes = 32;
        public const int BeforeNMBytes = 32;
        public const int NonceBytes = 24;
        public const int ZeroBytes = 32;
        public const int BoxZeroBytes = 16;
        public const int MACBytes = ZeroBytes - BoxZeroBytes;

        private static readonly byte[] Sigma = new byte[] { (byte)'e', (byte)'x', (byte)'p', 
            (byte)'a', (byte)'n', (byte)'d', (byte)' ', (byte)'3', (byte)'2', (byte)'-', 
            (byte)'b', (byte)'y', (byte)'t', (byte)'e', (byte)' ', (byte)'k' };

        private static readonly byte[] N = new byte[16];


        private static RNGCryptoServiceProvider s_random = new RNGCryptoServiceProvider();

        public void KeyPair(byte[] publickKey, byte[] secretKey)
        {
            s_random.GetBytes(secretKey);

            ScalarMultiplication.ScalarMultBase(publickKey, secretKey);
        }

        public void Create(byte[] cipher,
            byte[] message, int count,
            byte[] nonce,
            byte[] publickey,
            byte[] secretKey)
        {
            
        }

        public void Open(byte[] message, byte[] cipher, int count, byte[] nonce, byte[] publickey, byte[] secretKey)
        {
            
        }

        public void BeforeNM(byte[] k, byte[] pk, byte[] sk)
        {
            byte[]  s= new byte[32];
            ScalarMultiplication.ScalarMult(s, sk, pk);

            HSalsa20 salsa20 = new HSalsa20();
            salsa20.Transform(k, N, s, Sigma);
        }

        public void AfterNM(byte[] c, byte[] m, ulong mlen, byte[] n, byte[] k)
        {
            
        }

        public void OpenAfterNM(byte[] m, byte[] c, ulong clen, byte[] n, byte[] k)
        {
            
        }
    }
}
