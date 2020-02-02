using System;
using System.Security.Cryptography;
using NaCl.Internal;

namespace NaCl
{
    public class Curve25519XSalsa20Poly1305 : XSalsa20Poly1305
    {
        public const int SecretKeyLength = Curve25519.ScalarLength;
        public const int PublicKeyLength = Curve25519.ScalarLength;
        
        public Curve25519XSalsa20Poly1305(ReadOnlySpan<byte> secretKey, ReadOnlySpan<byte> publicKey)
        {
            ReadOnlySpan<byte> zero = stackalloc byte[16];
            Span<byte> s = stackalloc byte[32];

            Curve25519.ScalarMultiplication(s, secretKey, publicKey);

            HSalsa20.Transform(key, zero, s, Span<byte>.Empty);
            s.Clear();
        }

        public static void KeyPair(Span<byte> secretKey, Span<byte> publicKey)
        {
            if (secretKey.Length != Curve25519.ScalarLength)
                throw new ArgumentException("secretKey length must be 32 bytes");
            
            if (publicKey.Length != Curve25519.ScalarLength)
                throw new ArgumentException("publicKey length must be 32 bytes");

            using var rng = RandomNumberGenerator.Create();
#if NETSTANDARD2_1
            rng.GetBytes(secretKey);
#else
            var temp = new byte[KeyLength];
            rng.GetBytes(temp);
            for (int i = 0; i < KeyLength; i++)
                secretKey[i] = temp[i];
            Array.Clear(temp, 0, KeyLength);
            
#endif
            
            Curve25519.ScalarMultiplicationBase(publicKey, secretKey);
        }
        
        public static void KeyPair(byte[] secretKey, byte[] publicKey)
        { 
            KeyPair(new Span<byte>(secretKey), new Span<byte>(publicKey));
        }

        public static void KeyPair(out byte[] secretKey, out byte[] publicKey)
        {
            secretKey = new byte[SecretKeyLength];
            publicKey = new byte[PublicKeyLength];

            KeyPair(secretKey, publicKey);
        }
        
        public static (byte[], byte[]) KeyPair()
        {
            var secretKey = new byte[SecretKeyLength];
            var publicKey = new byte[PublicKeyLength];

            KeyPair(secretKey, publicKey);

            return (secretKey, publicKey);
        }
    }
}