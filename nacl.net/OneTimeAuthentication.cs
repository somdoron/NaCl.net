using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nacl
{
  public class OneTimeAuthentication
  {
    private Poly1305.Poly1305 m_poly1305;

    public const int OutpubSize = 16;
    public const int KeySize = 32;

    public OneTimeAuthentication()
    {
      m_poly1305 = Poly1305.Poly1305.Create();
    }
   
    public void Authenticate(byte[] output, byte[] input, byte[] key)
    {
      Authenticate(output,0, input,0, input.Length, key,0);
    }

    public void Authenticate(byte[] output, int outputOffset, byte[] input, int inputOffset, int inputLength, byte[] key,
     int keyOffset)
    {

      if (keyOffset == 0)
      {
        m_poly1305.Key = key;
      }
      else
      {
        byte []newKey = new byte[Poly1305.Poly1305.KeySize];
        Buffer.BlockCopy(key, keyOffset, newKey, 0, Poly1305.Poly1305.KeySize);

        m_poly1305.Key = newKey;
      }
      
      m_poly1305.Transform(input,inputOffset, inputLength);
      m_poly1305.Finish(output, outputOffset);
    }

    public bool Verify(byte[] hash, byte[] input, byte[] key)
    {
      return Verify(hash,0, input,0, input.Length, key,0);
    }

    public bool Verify(byte[] hash, int hashOffset, byte[] input,int inputOffset, int inputLength, byte[] key, int keyOffset)
    {
      if (keyOffset == 0)
      {
        m_poly1305.Key = key;
      }
      else
      {
        byte[] newKey = new byte[Poly1305.Poly1305.KeySize];
        Buffer.BlockCopy(key, keyOffset, newKey, 0, Poly1305.Poly1305.KeySize);

        m_poly1305.Key = newKey;
      }
   
      byte[] correct = new byte[16];
      
      m_poly1305.Transform(input,inputOffset, inputLength);
      m_poly1305.Finish(correct);

      return SafeComparison.Verify16(hash, hashOffset, correct,0);
    }
  }
}
