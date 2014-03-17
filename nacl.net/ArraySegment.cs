using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nacl
{


    public struct ArraySegment<T> : IEnumerable<T>
    {
        private readonly T[] m_array;
        private readonly int m_offset;
        private readonly int m_count;        

        public ArraySegment(T [] array, int offset, int count)
        {
            m_array = array;
            m_offset = offset;
            m_count = count;
        }

        public ArraySegment(T[] array, int offset) : this(array, offset, array.Length - offset)
        {
            
        }

        public ArraySegment(T[] array) : this(array, 0, array.Length)
        {
            
        }

        public ArraySegment(int size)
            : this(new T[size])
        {

        }

        public int Count
        {
            get { return m_count; }
        }

        public static implicit operator ArraySegment<T>(T[] array)
        {
            return new ArraySegment<T>(array);
        }   

        public static ArraySegment<T> operator +(ArraySegment<T> array, int num)
        {
            return new ArraySegment<T>(array.m_array, array.m_offset + num, array.Count - num);
        }

        public static ArraySegment<T> operator -(ArraySegment<T> array, int num)
        {
            return new ArraySegment<T>(array.m_array, array.m_offset - num, array.Count + num);
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
