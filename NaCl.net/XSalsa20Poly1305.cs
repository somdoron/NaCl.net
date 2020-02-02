using System;
using NaCl.Internal;

namespace NaCl
{
    public class XSalsa20Poly1305 : IDisposable
    {
        private const int ZeroBytesLength = 32;

        public const int KeyLength = 32;
        public const int MacLength = Poly1305.HashLength;
        public const int NonceLength = 24;

        private Poly1305 poly1305;
        protected internal byte[] key;

        public XSalsa20Poly1305(ReadOnlySpan<byte> key)
        {
            if (key.Length != KeyLength)
                throw new ArgumentException("key length must be 32 bytes");
            this.key = key.ToArray();
            poly1305 = new Poly1305();
        }

        public XSalsa20Poly1305(byte[] key)
        {
            if (key.Length != KeyLength)
                throw new ArgumentException("key length must be 32 bytes");
            this.key = (byte[]) key.Clone();
            poly1305 = new Poly1305();
        }

        protected XSalsa20Poly1305()
        {
            poly1305 = new Poly1305();
            this.key = new byte[KeyLength];
        }

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
        
        public void Encrypt(byte[] cipher, int cipherOffset, byte[] mac, int macOffset, byte[] message,
            int messageOffset, int messageCount,
            byte[] nonce, int nonceOffset)
        {
            Encrypt(new Span<byte>(cipher, cipherOffset, messageCount),
                new Span<byte>(mac, macOffset, MacLength), new ReadOnlySpan<byte>(message, messageOffset, messageCount), new ReadOnlySpan<byte>(nonce, nonceOffset, NonceLength));
        }


        public void Encrypt(Span<byte> cipher, ReadOnlySpan<byte> message, ReadOnlySpan<byte> nonce)
        {
            Encrypt(cipher.Slice(MacLength), cipher.Slice(0, MacLength), message, nonce);
        }
        
        public void Encrypt(byte[] cipher, int cipherOffset, byte[] message, int messageOffset, int messageCount,
            byte[] nonce, int nonceOffset)
        {
            Encrypt(new Span<byte>(cipher, cipherOffset + MacLength, messageCount),
                new Span<byte>(cipher, cipherOffset, MacLength), new ReadOnlySpan<byte>(message, messageOffset, messageCount), new ReadOnlySpan<byte>(nonce, nonceOffset, NonceLength));
        }

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

        public bool TryDecrypt(byte[] message, int messageOffset, byte[] cipher, int cipherOffset, int cipherCount,
            byte[] mac, int macOffset,
            byte[] nonce, int nonceOffset)
        {
            return TryDecrypt(new Span<byte>(message, messageOffset, cipherCount), new ReadOnlySpan<byte>(cipher, cipherOffset, cipherCount), new ReadOnlySpan<byte>(mac, macOffset, MacLength), new ReadOnlySpan<byte>(nonce, nonceOffset, NonceLength));
        }

        public bool TryDecrypt(Span<byte> message, ReadOnlySpan<byte> cipher, ReadOnlySpan<byte> nonce)
        {
            return TryDecrypt(message, cipher.Slice(MacLength), cipher.Slice(0, MacLength), nonce);
        }

        public bool TryDecrypt(byte[] message, int messageOffset, byte[] cipher, int cipherOffset, int cipherCount,
            byte[] nonce, int nonceOffset)
        {
            return TryDecrypt(new Span<byte>(message, messageOffset, cipherCount - MacLength), new ReadOnlySpan<byte>(cipher, cipherOffset, cipherCount), new ReadOnlySpan<byte>(nonce, nonceOffset, NonceLength));
        }

        public void Dispose()
        {
            Array.Clear(key, 0, key.Length);
            poly1305.Dispose();
        }
    }
}