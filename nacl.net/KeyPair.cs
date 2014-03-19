using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nacl
{
  public class KeyPair
  {
    public byte[] PublicKey { get;set; }
    public byte[] SecretKey { get; set; }

    public KeyPair()
    {
      
    }

    public KeyPair(byte[] publicKey, byte[] secretKey)
    {
      PublicKey = publicKey;
      SecretKey = secretKey;
    }
  }
}
