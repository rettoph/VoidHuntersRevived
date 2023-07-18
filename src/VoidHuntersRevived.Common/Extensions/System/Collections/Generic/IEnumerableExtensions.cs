using Microsoft.Xna.Framework;
using Svelto.Common;
using Svelto.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public static class IEnumerableExtensions
    {
        public static Vector2 Average(this IEnumerable<Vector2> values)
        {
            int count = 0;
            Vector2 sum = Vector2.Zero;

            foreach(Vector2 vector in values)
            {
                sum += vector;
                count++;
            }

            return sum / count;
        }

        public static NativeDynamicArrayCast<T> ToNativeDynamicArray<T>(this IEnumerable<T> items, Allocator allocator = Allocator.Persistent)
            where T : struct
        {
            NativeDynamicArrayCast<T> native = new NativeDynamicArrayCast<T>((uint)items.Count(), allocator);

            int index = 0;
            foreach(T item in items)
            {
                native.Set(index++, item);
            }

            return native;
        }
    }
}
