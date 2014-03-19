using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace nacl
{
  internal struct ArraySegment<T> : IEnumerable<T>
  {
    private readonly T[] m_array;
    private readonly int m_offset;    

    public ArraySegment(T[] array, int offset)
    {
      m_array = array;
      m_offset = offset;      
    }    

    public ArraySegment(T[] array)
      : this(array, 0)
    {

    }

    public ArraySegment(int size)
      : this(new T[size])
    {

    }

    public int Count
    {
      get
      {
        if (m_array == null)
        {
          return 0;
        }
        return m_array.Length;
      }
    }

    public int Offset
    {
      get
      {
        return m_offset;
      }
    }

    public T[] InnerArray
    {
      get
      {
        return m_array;
      }
    }

    public T[] ToArray()
    {
      T[] array = new T[Count];

      Array.Copy(m_array, m_offset, array, 0, Count);

      return array;
    }   

    public static implicit operator ArraySegment<T>(T[] array)
    {
      return new ArraySegment<T>(array);
    }

    public static ArraySegment<T> operator +(ArraySegment<T> array, int num)
    {
      return new ArraySegment<T>(array.m_array, array.m_offset + num);
    }

    public static ArraySegment<T> operator -(ArraySegment<T> array, int num)
    {
      return new ArraySegment<T>(array.m_array, array.m_offset - num);
    }

    public T this[uint i]
    {
      get
      {
        if (i > Count - 1)
        {
          throw new ArgumentOutOfRangeException("i");
        }

        return m_array[i + m_offset];
      }
      set
      {
        if (i > Count - 1)
        {
          throw new ArgumentOutOfRangeException("i");
        }

        m_array[i + m_offset] = value;
      }
    }

    public T this[int i]
    {
      get
      {
        if (i < 0 || i > Count - 1)
        {
          throw new ArgumentOutOfRangeException("i");
        }

        return m_array[i + m_offset];
      }
      set
      {
        if (i < 0 || i > Count - 1)
        {
          throw new ArgumentOutOfRangeException("i");
        }

        m_array[i + m_offset] = value;
      }
    }

    public IEnumerator<T> GetEnumerator()
    {
      for (int i = 0; i < Count; i++)
      {
        yield return m_array[i + m_offset];
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      for (int i = 0; i < Count; i++)
      {
        yield return m_array[i + m_offset];
      }
    }
  }
}
