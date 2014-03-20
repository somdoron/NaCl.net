using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace nacl
{
  public enum SecretAuthenticationPrimitive
  {
    HMACSHA256,HMACSHA512256
  }

  public class SecretAuthentication
  {    
    private HMAC m_hmac;    

    public SecretAuthentication(SecretAuthenticationPrimitive primitive)
    {
      Primitive = primitive;

      if (primitive == SecretAuthenticationPrimitive.HMACSHA512256)
      {
        m_hmac = new HMACSHA512();  
      }
      else
      {
       m_hmac = new HMACSHA256(); 
      }
    }

    public SecretAuthenticationPrimitive Primitive { get; private set; }

    public const int OutputSize = 32;
    public const int KeyBytes = 32;

    public void Authenticate(byte[] output, byte[] message, byte[] key)
    {
      Authenticate(output, 0, message, 0, message.Length, key);
    }

    public void Authenticate(byte[] output, int outputOffset, byte[] message, int messageOffset, int messageLength, byte[] key)
    {
      m_hmac.Key = key;
      byte[] hash = m_hmac.ComputeHash(message, messageOffset, messageLength);

      Buffer.BlockCopy(hash, 0, output, outputOffset, OutputSize);      
    }

    public bool Verify(byte[] mac, byte[] message, byte[] key)
    {
      return Verify(mac, 0, message, 0, message.Length, key);
    }

    public bool Verify(byte[] mac, int macOffset, byte[] message, int messageOffset, int messageLength, byte[] key)
    {
      m_hmac.Key = key;

      byte[] mac2 = m_hmac.ComputeHash(message, messageOffset, messageLength);

      return SafeComparison.Verify32((ArraySegment<byte>) mac + macOffset, mac2);
    }

  }
}
