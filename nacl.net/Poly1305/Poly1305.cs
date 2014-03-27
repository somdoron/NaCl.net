using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

    private byte[] m_key;
    private int m_keyOffset;

    protected abstract void Blocks(byte[] m, int offset, int count);

    protected abstract void OnFinish(byte[] mac, int offset);

    protected abstract void OnReset();

    protected abstract void OnKeyChanged();

    public int KeyOffset
    {
      get
      {
        return m_keyOffset;
      }
    }

    public byte[] Key
    {
      get { return m_key; }
    }

    public void SetKey(byte[] key, int keyOffset)
    {   
      if (key.Length - keyOffset < KeySize)
      {
        throw new ArgumentException("value size must be greater or equal to " + KeySize.ToString());
      }

      Reset();

      m_key = key;
      m_keyOffset = keyOffset;
      OnKeyChanged();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void Reset()
    {
      m_leftover = 0;
      m_final = false;

      Array.Clear(m_buffer, 0, m_buffer.Length);

      OnReset();
    }

    public void Finish(byte[] mac, int offset = 0)
    {
      OnFinish(mac, offset);
      Reset();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Transform(byte[] message, int offset, int count)
    {
      int i;

      /* handle leftover */
      if (m_leftover != 0)
      {
        int want = (BlockSize - m_leftover);
        if (want > count)
          want = count;
        for (i = 0; i < want; i++)
          m_buffer[m_leftover + i] = message[i + offset];
        count -= want;
        offset += want;
        m_leftover += want;
        if (m_leftover < BlockSize)
          return;
        Blocks(m_buffer, 0, BlockSize);
        m_leftover = 0;
      }

      /* process full blocks */
      if (count >= BlockSize)
      {
        int want = (count & ~(BlockSize - 1));
        Blocks(message, offset, want);
        offset += want;
        count -= want;
      }

      /* store leftover */
      if (count != 0)
      {
        for (i = 0; i < count; i++)
          m_buffer[m_leftover + i] = message[i + offset];
        m_leftover += count;
      }
    }

    public static Poly1305 Create()
    {
      // we alawys pick 32bit because it much faster
      return new Poly1305_32Bit();
    }
  }
}
