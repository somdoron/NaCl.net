using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nacl.Poly1305
{
  abstract class Poly1305
  {
    public const int BlockSize = 16;

    protected abstract void Blocks(ArraySegment<byte> m, int count);

    public abstract void Finish(ArraySegment<byte> mac);

    public abstract void Init(ArraySegment<byte> key);
    

    protected int m_leftover;
    protected byte[] m_buffer = new byte[BlockSize];

    public void Transform(ArraySegment<byte> message, int count)
    {
      int i;

      /* handle leftover */
      if (m_leftover != 0)
      {
        int want = (BlockSize - m_leftover);
        if (want > count)
          want = count;
        for (i = 0; i < want; i++)
          m_buffer[m_leftover + i] = message[i];
        count -= want;
        message += want;
        m_leftover += want;
        if (m_leftover < BlockSize)
          return;
        Blocks(m_buffer, BlockSize);
        m_leftover = 0;
      }

      /* process full blocks */
      if (count >= BlockSize)
      {
        int want = (count & ~(BlockSize - 1));
        Blocks(message, want);
        message += want;
        count -= want;
      }

      /* store leftover */
      if (count != 0)
      {
        for (i = 0; i < count; i++)
          m_buffer[m_leftover + i] = message[i];
        m_leftover += count;
      }
    }

    public static Poly1305 Create()
    {
      return new Poly1305_32Bit();
    }

    public static void Auth(ArraySegment<byte> mac, ArraySegment<byte> message, int count, ArraySegment<byte> key)
    {
      var poly1305 = Create();
      poly1305.Init(key);
      poly1305.Transform(message, count);
      poly1305.Finish(mac);
    }

    public static bool Verify(ArraySegment<byte> mac1, ArraySegment<byte> mac2)
    {      
      uint dif = 0;
      for (int i = 0; i < 16; i++)
        dif |= (uint)((mac1[i] ^ mac2[i]));
            
      dif = (dif - 1) >> ((4 * 8) - 1);
	    
      return (dif & 1) == 1;
    }
  }
}
