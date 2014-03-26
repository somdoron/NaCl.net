namespace nacl.Stream
{
  abstract class BaseStream
  {
    public abstract int KeySize { get; }


    public abstract int NonceSize { get; }

    public void Transform(byte[] cipher, int cipherLength, byte[] nonce, byte[] key)
    {
      Transform(cipher, 0 ,cipherLength, nonce, 0, key);
    }

    public void TransformXor(byte[] cipher, byte[] message, int messageLength, byte[] nonce, byte[] key)
    {
      TransformXor(cipher,0, message,0, messageLength, nonce, 0, key);
    }

    public abstract void Transform(byte[] cipher,int cipherOffset, int cipherLength, byte[] nonce, int nonceOffset, byte[] key);

    public abstract void TransformXor(byte[] cipher,int cipherOffset, byte[] message,int messageOffset, int messageLength, byte[] nonce, int nonceOffset, byte[] key);
    
  }
}