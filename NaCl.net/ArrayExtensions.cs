using System;
using System.Runtime.CompilerServices;

namespace NetMQ.Utils
{
    internal static class ArrayExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear<T>(this T[] array)
        {
            if (array != null)
            {
                Array.Clear(array, 0, array.Length);
            }
        }
    }
}