using System;
using System.Security.Cryptography;
using NaCl.Internal;

namespace NaCl
{
    /// <summary>
    /// XSalsa20 is a stream cipher based upon Salsa20 but with a much longer nonce: 192 bits instead of 64 bits.
    /// </summary>
    /// <remarks>
    /// <para>XSalsa20 uses a 256-bit key as well as the first 128 bits of the nonce in order to compute a subkey.
    /// This subkey, as well as the remaining 64 bits of the nonce, are the parameters of the Salsa20 function
    /// used to actually generate the stream.
    /// </para>
    /// <para>
    /// Like Salsa20, XSalsa20 is immune to timing attacks and provides its own 64-bit block counter to avoid
    /// incrementing the nonce after each block.
    /// </para>
    /// <para>
    /// But with XSalsa20's longer nonce, it is safe to generate nonces using
    /// <see cref="RandomNumberGenerator"/> for every message encrypted with the same key without having to
    /// worry about a collision.
    /// </para>
    /// </remarks>
    public class XSalsa20 : IDisposable
    {
        /// <summary>
        /// The key length, 32 bytes.
        /// </summary>
        public const int KeyLength = 32;
        
        /// <summary>
        /// The nonce length, 24 bytes.
        /// </summary>
        public const int NonceLength = 24;

        private byte[] key;

        /// <summary>
        /// Create a new XSalsa object with the specified key
        /// </summary>
        /// <param name="key">The key</param>
        /// <exception cref="ArgumentException">Thrown if key length is not 32 bytes</exception>
        public XSalsa20(ReadOnlySpan<byte> key)
        {
            if (key.Length != KeyLength)
                throw new ArgumentException("key length must be 32 bytes");
            this.key = key.ToArray();
        }

        /// <summary>
        /// Create a new XSalsa object with the specified key
        /// </summary>
        /// <param name="key">The key</param>
        /// <exception cref="ArgumentException">Thrown if key length is not 32 bytes</exception>
        public XSalsa20(byte[] key)
        {
            if (key.Length != KeyLength)
                throw new ArgumentException("key length must be 32 bytes");
            this.key = (byte[]) key.Clone();
        }
        
        /// <summary>
        /// Dispose the object and clear any sensitive data.
        /// </summary>
        public void Dispose()
        {
            Array.Clear(key, 0, 32);
        }
        
        /// <summary>
        /// Transform a message using a nonce and a secret key.
        /// </summary>
        /// <param name="output">Output will be written to the parameter.</param>
        /// <param name="input">Input to transform</param>
        /// <param name="nonce">Nonce</param>
        public void Transform(Span<byte> output, ReadOnlySpan<byte> input, ReadOnlySpan<byte> nonce)
        {
            Span<byte> subkey = stackalloc byte[32];

            HSalsa20.Transform(subkey, nonce, key, Span<byte>.Empty);
            StreamSalsa20Xor.Transform(output, input, nonce.Slice(16), subkey, 0U);
        }

        /// <summary>
        /// Transform a message using a nonce and a secret key.
        /// </summary>
        /// <param name="output">Output will be written to the parameter.</param>
        /// <param name="outputOffset">Offset to start write to</param>
        /// <param name="input">Input to transform</param>
        /// <param name="inputOffset">Offset to start read from</param>
        /// <param name="inputCount">Amount of bytes to read</param>
        /// <param name="nonce">Nonce</param>
        /// <param name="nonceOffset">Nonce offset</param>
        public void Transform(byte[] output, int outputOffset, byte[] input, int inputOffset, int inputCount,
            byte[] nonce, int nonceOffset)
        {
            Transform(new Span<byte>(output, outputOffset, inputCount),
                new ReadOnlySpan<byte>(input, inputOffset, inputCount), 
                new ReadOnlySpan<byte>(nonce, nonceOffset, NonceLength));
        }
    }
}