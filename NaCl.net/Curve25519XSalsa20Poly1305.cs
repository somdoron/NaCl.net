using System;
using System.Security.Cryptography;
using NaCl.Internal;

namespace NaCl
{
    /// <summary>
    /// Public-key authenticated encryption.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Using public-key authenticated encryption, Bob can encrypt a confidential message specifically for Alice,
    /// using Alice's public key.</para>
    /// <para>Using Bob's public key, Alice can compute a shared secret key. Using Alice's public key and his secret key,
    /// Bob can compute the exact same shared secret key.
    /// That shared secret key can be used to verify that the encrypted message was not tampered with,
    /// before eventually decrypting it.</para>
    /// <para>Alice only needs Bob's public key, the nonce and the ciphertext. Bob should never ever share his secret key,
    /// even with Alice.</para>
    /// <para>And in order to send messages to Alice, Bob only needs Alice's public key.
    /// Alice should never ever share her secret key either, even with Bob.</para>
    /// <para>Alice can reply to Bob using the same system, without having to generate a distinct key pair.</para>
    /// <para>The nonce doesn't have to be confidential, but it should be used with just one invocation of <see>
    ///     <cref>Curve25519XSalsa20Poly1305.Encrypt</cref>
    /// </see> for a particular pair of public and secret keys.</para>
    /// <para>One easy way to generate a nonce is to use <see cref="RandomNumberGenerator"/>,
    /// considering the size of the nonces the risk of any random collisions is negligible.
    /// For some applications, if you wish to use nonces to detect missing messages or to ignore replayed messages,
    /// it is also acceptable to use a simple incrementing counter as a nonce.</para>
    /// <para>When doing so you must ensure that the same value can never be re-used
    /// (for example you may have multiple threads or even hosts generating messages using the same key pairs).</para>
    /// <para>As stated above, senders can decrypt their own messages, and compute a valid authentication tag for any
    /// messages encrypted with a given shared secret key. This is generally not an issue for online protocols.
    /// If this is not acceptable, check out the Sealed Boxes section,
    /// as well as the Key Exchange section in this documentation.</para>
    /// </remarks>
    public class Curve25519XSalsa20Poly1305 : XSalsa20Poly1305
    {
        /// <summary>
        /// Length of the secret key, 32.
        /// </summary>
        public const int SecretKeyLength = Curve25519.ScalarLength;
        
        /// <summary>
        /// Length of the public key, 32.
        /// </summary>
        public const int PublicKeyLength = Curve25519.ScalarLength;

        /// <summary>
        /// Create a new Curve25519XSalsa20Poly1305 and pre-calculate the shared secret from secret and public key.
        /// </summary>
        /// <param name="secretKey">SecretKey</param>
        /// <param name="publicKey">PublicKey</param>
        public Curve25519XSalsa20Poly1305(ReadOnlySpan<byte> secretKey, ReadOnlySpan<byte> publicKey)
        {
            ReadOnlySpan<byte> zero = stackalloc byte[16];
            Span<byte> s = stackalloc byte[32];

            Curve25519.ScalarMultiplication(s, secretKey, publicKey);

            HSalsa20.Transform(key, zero, s, Span<byte>.Empty);
            s.Clear();
        }

        /// <summary>
        /// Randomly generates a secret key and a corresponding public key.
        /// </summary>
        /// <param name="secretKey">Buffer the secret key will be written to.</param>
        /// <param name="publicKey">Buffer the public key will be written to.</param>
        /// <exception cref="ArgumentException">thrown if secretKey or publicKey are not 32 bytes long</exception>
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

        
        /// <summary>
        /// Randomly generates a secret key and a corresponding public key.
        /// </summary>
        /// <param name="secretKey">Buffer the secret key will be written to.</param>
        /// <param name="publicKey">Buffer the public key will be written to.</param>
        /// <exception cref="ArgumentException">thrown if secretKey or publicKey are not 32 bytes long</exception>
        public static void KeyPair(byte[] secretKey, byte[] publicKey)
        {
            KeyPair(new Span<byte>(secretKey), new Span<byte>(publicKey));
        }

        /// <summary>
        /// Randomly generates a secret key and a corresponding public key.
        /// </summary>
        /// <param name="secretKey">Generated secret-key.</param>
        /// <param name="publicKey">Corresponding public key</param>
        public static void KeyPair(out byte[] secretKey, out byte[] publicKey)
        {
            secretKey = new byte[SecretKeyLength];
            publicKey = new byte[PublicKeyLength];

            KeyPair(secretKey, publicKey);
        }

        /// <summary>
        /// Randomly generates a secret key and a corresponding public key.
        /// </summary>
        /// <returns>Returns a pair of secret-key and public-key</returns>
        public static (byte[], byte[]) KeyPair()
        {
            var secretKey = new byte[SecretKeyLength];
            var publicKey = new byte[PublicKeyLength];

            KeyPair(secretKey, publicKey);

            return (secretKey, publicKey);
        }
    }
}