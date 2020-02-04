using System;
using System.Security.Cryptography;
using NaCl.Internal;

namespace NaCl
{
    /// <summary>
    /// Encrypts a message with a key and a nonce to keep it confidential and
    /// Computes an authentication tag. This tag is used to make sure that the message
    /// hasn't been tampered with before decrypting it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A single key is used both to encrypt/authenticate and verify/decrypt messages.
    /// For this reason, it is critical to keep the key confidential.
    /// </para>
    /// <para>
    /// The nonce doesn't have to be confidential, but it should never ever be reused with the same key.
    /// The easiest way to generate a nonce is to use <see cref="RandomNumberGenerator"/>.
    /// </para>
    /// <para>
    /// Messages encrypted are assumed to be independent.
    /// If multiple messages are sent using this API and random nonces,
    /// there will be no way to detect if a message has been received twice,
    /// or if messages have been reordered.
    /// </para>
    /// </remarks>
    public class XSalsa20Poly1305 : IDisposable
    {
        private const int ZeroBytesLength = 32;

        /// <summary>
        /// Key length, 32 bytes.
        /// </summary>
        public const int KeyLength = 32;
        
        /// <summary>
        /// Tag length, 16 bytes.
        /// </summary>
        public const int TagLength = Poly1305.TagLength;
        
        /// <summary>
        /// Nonce length, 24 bytes.
        /// </summary>
        public const int NonceLength = 24;

        private Poly1305 poly1305;
        internal byte[] key;

        /// <summary>
        /// Create a new object with the specified shared key
        /// </summary>
        /// <param name="key">Shared key</param>
        /// <exception cref="ArgumentException">Thrown if key is not 32 bytes long</exception>
        public XSalsa20Poly1305(ReadOnlySpan<byte> key)
        {
            if (key.Length != KeyLength)
                throw new ArgumentException("key length must be 32 bytes");
            this.key = key.ToArray();
            poly1305 = new Poly1305();
        }

        /// <summary>
        /// Create a new object with the specified shared key
        /// </summary>
        /// <param name="key">Shared key</param>
        /// <exception cref="ArgumentException">Thrown if key is not 32 bytes long</exception>
        public XSalsa20Poly1305(byte[] key)
        {
            if (key.Length != KeyLength)
                throw new ArgumentException("key length must be 32 bytes");
            this.key = (byte[]) key.Clone();
            poly1305 = new Poly1305();
        }

        internal XSalsa20Poly1305()
        {
            poly1305 = new Poly1305();
            this.key = new byte[KeyLength];
        }

        /// <summary>
        /// Encrypts a message, with the object key and a nonce n.
        /// </summary>
        /// <remarks>
        /// Detached mode, some applications may need to store the authentication tag and
        /// the encrypted message at different locations.
        /// </remarks>
        /// <param name="cipher"></param>
        /// <param name="mac"></param>
        /// <param name="message"></param>
        /// <param name="nonce"></param>
        public void Encrypt(Span<byte> cipher, Span<byte> mac, ReadOnlySpan<byte> message, ReadOnlySpan<byte> nonce)
        {
            Span<byte> block0 = stackalloc byte[64];
            Span<byte> subkey = stackalloc byte[Salsa20.KeyLength];

            HSalsa20.Transform(subkey, nonce, key, ReadOnlySpan<byte>.Empty);

            int mlen0 = message.Length;
            if (mlen0 > 64 - ZeroBytesLength)
                mlen0 = 64 - ZeroBytesLength;

            for (int i = 0; i < mlen0; i++)
                block0[i + ZeroBytesLength] = message[i];

            StreamSalsa20Xor.Transform(block0, block0.Slice(0, mlen0 + ZeroBytesLength), nonce.Slice(16), subkey);

            poly1305.SetKey(block0.Slice(0, Poly1305.KeyLength));
            for (int i = 0; i < mlen0; i++)
                cipher[i] = block0[ZeroBytesLength + i];
            block0.Clear();

            if (message.Length > mlen0)
                StreamSalsa20Xor.Transform(cipher.Slice(mlen0), message.Slice(mlen0), nonce.Slice(16), subkey, 1UL);
            subkey.Clear();

            poly1305.Update(cipher.Slice(0, message.Length));
            poly1305.Final(mac);
            poly1305.Reset();
        }
        
        /// <summary>
        /// Encrypts a message, with the object key and a nonce n.
        /// </summary>
        /// <remarks>
        /// Detached mode, some applications may need to store the authentication tag and
        /// the encrypted message at different locations.
        /// </remarks>
        public void Encrypt(byte[] cipher, int cipherOffset, byte[] mac, int macOffset, byte[] message,
            int messageOffset, int messageCount,
            byte[] nonce, int nonceOffset)
        {
            Encrypt(new Span<byte>(cipher, cipherOffset, messageCount),
                new Span<byte>(mac, macOffset, TagLength), new ReadOnlySpan<byte>(message, messageOffset, messageCount), new ReadOnlySpan<byte>(nonce, nonceOffset, NonceLength));
        }

        /// <summary>
        /// Encrypts a message, with the object key and a nonce n.
        /// </summary>
        /// <remarks>
        /// Combined mode, the authentication tag and the encrypted message are stored together.
        /// This is usually what you want.
        /// </remarks>
        /// <param name="cipher">Encrypted text will be written to the buffer</param>
        /// <param name="message">Message to encrypt</param>
        /// <param name="nonce">The nonce</param>
        public void Encrypt(Span<byte> cipher, ReadOnlySpan<byte> message, ReadOnlySpan<byte> nonce)
        {
            Encrypt(cipher.Slice(TagLength), cipher.Slice(0, TagLength), message, nonce);
        }

        /// <summary>
        /// Encrypts a message, with the object key and a nonce n.
        /// </summary>
        /// <remarks>
        /// Combined mode, the authentication tag and the encrypted message are stored together.
        /// This is usually what you want.
        /// </remarks>
        /// <param name="cipher">Encrypted text will be written to the buffer</param>
        /// <param name="cipherOffset">Offset to start write the cipher text to</param>
        /// <param name="message">Message to encrypt</param>
        /// <param name="messageOffset">Offset to start read message from</param>
        /// <param name="messageCount">Number of bytes to read from message</param>
        /// <param name="nonce">The nonce</param>
        /// <param name="nonceOffset">Nonce offset</param>
        public void Encrypt(byte[] cipher, int cipherOffset, byte[] message, int messageOffset, int messageCount,
            byte[] nonce, int nonceOffset)
        {
            Encrypt(new Span<byte>(cipher, cipherOffset + TagLength, messageCount),
                new Span<byte>(cipher, cipherOffset, TagLength), new ReadOnlySpan<byte>(message, messageOffset, messageCount), new ReadOnlySpan<byte>(nonce, nonceOffset, NonceLength));
        }

        /// <summary>
        /// Verifies and decrypts a ciphertext produced by <see>
        ///     <cref>Encrypt</cref>
        /// </see>
        /// </summary>
        /// <remarks>
        /// Detached mode, some applications may need to store the authentication tag and
        /// the encrypted message at different locations.
        /// </remarks>
        /// <returns>True if successfully verified and decrypted ciphertext.</returns>
        public bool TryDecrypt(Span<byte> message, ReadOnlySpan<byte> cipher, ReadOnlySpan<byte> mac,
            ReadOnlySpan<byte> nonce)
        {
            Span<byte> block0 = stackalloc byte[64];
            Span<byte> subkey = stackalloc byte[Salsa20.KeyLength];

            HSalsa20.Transform(subkey, nonce, key, ReadOnlySpan<byte>.Empty);
            StreamSalsa20.Transform(block0.Slice(0, StreamSalsa20.KeyLength), nonce.Slice(16), subkey);

            poly1305.SetKey(block0.Slice(0, Poly1305.KeyLength));
            if (!poly1305.Verify(mac, cipher))
            {
                poly1305.Reset();
                subkey.Clear();
                return false;
            }

            int mlen0 = cipher.Length;
            if (mlen0 > 64 - ZeroBytesLength)
                mlen0 = 64 - ZeroBytesLength;

            for (int i = 0; i < mlen0; i++)
                block0[ZeroBytesLength + i] = cipher[i];

            StreamSalsa20Xor.Transform(block0, block0.Slice(0, ZeroBytesLength + mlen0), nonce.Slice(16), subkey);
            for (int i = 0; i < mlen0; i++)
                message[i] = block0[i + ZeroBytesLength];

            if (cipher.Length > mlen0)
                StreamSalsa20Xor.Transform(message.Slice(mlen0), cipher.Slice(mlen0), nonce.Slice(16), subkey, 1UL);

            subkey.Clear();
            poly1305.Reset();
            return true;
        }

        /// <summary>
        /// Verifies and decrypts a ciphertext produced by <see>
        ///     <cref>Encrypt</cref>
        /// </see>
        /// </summary>
        /// <remarks>
        /// Detached mode, some applications may need to store the authentication tag and
        /// the encrypted message at different locations.
        /// </remarks>
        public bool TryDecrypt(byte[] message, int messageOffset, byte[] cipher, int cipherOffset, int cipherCount,
            byte[] mac, int macOffset,
            byte[] nonce, int nonceOffset)
        {
            return TryDecrypt(new Span<byte>(message, messageOffset, cipherCount), new ReadOnlySpan<byte>(cipher, cipherOffset, cipherCount), new ReadOnlySpan<byte>(mac, macOffset, TagLength), new ReadOnlySpan<byte>(nonce, nonceOffset, NonceLength));
        }

        /// <summary>
        /// Verifies and decrypts a ciphertext produced by <see>
        ///     <cref>Encrypt</cref>
        /// </see>
        /// </summary>
        /// <remarks>
        /// Combined mode, the authentication tag and the encrypted message are stored together.
        /// This is usually what you want.
        /// </remarks>
        /// <param name="message"></param>
        /// <param name="cipher"></param>
        /// <param name="nonce"></param>
        /// <returns>True if successfully verified and decrypted ciphertext.</returns>
        public bool TryDecrypt(Span<byte> message, ReadOnlySpan<byte> cipher, ReadOnlySpan<byte> nonce)
        {
            return TryDecrypt(message, cipher.Slice(TagLength), cipher.Slice(0, TagLength), nonce);
        }


        /// <summary>
        /// Verifies and decrypts a ciphertext produced by <see>
        ///     <cref>Encrypt</cref>
        /// </see>
        /// </summary>
        /// <remarks>
        /// Combined mode, the authentication tag and the encrypted message are stored together.
        /// This is usually what you want.
        /// </remarks>
        /// <param name="message"></param>
        /// <param name="messageOffset"></param>
        /// <param name="cipher"></param>
        /// <param name="cipherOffset"></param>
        /// <param name="cipherCount"></param>
        /// <param name="nonce"></param>
        /// <param name="nonceOffset"></param>
        /// <returns>True if successfully verified and decrypted ciphertext.</returns>
        public bool TryDecrypt(byte[] message, int messageOffset, byte[] cipher, int cipherOffset, int cipherCount,
            byte[] nonce, int nonceOffset)
        {
            return TryDecrypt(new Span<byte>(message, messageOffset, cipherCount - TagLength), new ReadOnlySpan<byte>(cipher, cipherOffset, cipherCount), new ReadOnlySpan<byte>(nonce, nonceOffset, NonceLength));
        }

        /// <summary>
        /// Dispose the object and clear any sensitive information
        /// </summary>
        public void Dispose()
        {
            Array.Clear(key, 0, key.Length);
            poly1305.Dispose();
        }
    }
}