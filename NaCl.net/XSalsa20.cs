using System;
using NaCl.Internal;

namespace NaCl
{
    public class XSalsa20 : IDisposable
    {
        public const int KeyLength = 32;
        public const int NonceLength = 24;

        private byte[] key;

        public XSalsa20(ReadOnlySpan<byte> key)
        {
            if (key.Length != KeyLength)
                throw new ArgumentException("key length must be 32 bytes");
            this.key = key.ToArray();
        }

        public XSalsa20(byte[] key)
        {
            if (key.Length != KeyLength)
                throw new ArgumentException("key length must be 32 bytes");
            this.key = (byte[]) key.Clone();
        }
        
        public void Dispose()
        {
            Array.Clear(key, 0, 32);
        }
        
        public void Transform(Span<byte> cipher, ReadOnlySpan<byte> message, ReadOnlySpan<byte> nonce)
        {
            Span<byte> subkey = stackalloc byte[32];

            HSalsa20.Transform(subkey, nonce, key, Span<byte>.Empty);
            StreamSalsa20Xor.Transform(cipher, message, nonce.Slice(16), subkey, 0U);
        }
        
        public void Transform(byte[] cipher, int cipherOffset, byte[] message, int messageOffset, int messageCount,
            byte[] nonce, int nonceOffset)
        {
            Transform(new Span<byte>(cipher, cipherOffset, messageCount),
                new ReadOnlySpan<byte>(message, messageOffset, messageCount), 
                new ReadOnlySpan<byte>(nonce, nonceOffset, NonceLength));
        }
    }
}