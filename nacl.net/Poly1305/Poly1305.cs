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

    public const int KeySize = 32;

    protected int m_leftover;
    protected byte[] m_buffer = new byte[BlockSize];
    protected bool m_final;

    private ArraySegment<byte> m_key;

    protected abstract void Blocks(ArraySegment<byte> m, int count);

    protected abstract void OnFinish(ArraySegment<byte> mac);

    protected abstract void OnReset();

    protected abstract void OnKeyChanged();

    public ArraySegment<byte> Key
    {
      get { return m_key; }
      set
      {
        bool equal = true;

        if (value.Count < KeySize)
        {
          throw new ArgumentException("value size must be greater or equal to " + KeySize.ToString());
        }

        Reset();

        if (m_key.Count < KeySize)
        {
          equal = false;
        }
        else
        {
          for (int i = 0; i < KeySize; i++)
          {
            if (Key[i] != value[i])
            {
              equal = false;
              break;
            }
          }
        }

        if (!equal)
        {
          m_key = value;
          OnKeyChanged();
        }
      }
    }

    public virtual void Reset()
    {
      m_leftover = 0;
      m_final = false;

      Array.Clear(m_buffer, 0, m_buffer.Length);

      OnReset();
    }

    public void Finish(ArraySegment<byte> mac)
    {
      OnFinish(mac);
      Reset();
    }

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
  }
}
