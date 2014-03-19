namespace nacl.Stream
{
  abstract class BaseStream
  {
    public abstract int KeySize { get; }


    public abstract int NonceSize { get; }


    public abstract void Transform(ArraySegment<byte> cipher, int cipherLength,
      ArraySegment<byte> nonce, ArraySegment<byte> key);

    public abstract void TransformXor(ArraySegment<byte> cipher, ArraySegment<byte> message, int messageLength,
      ArraySegment<byte> nonce, ArraySegment<byte> key);
    
  }
}