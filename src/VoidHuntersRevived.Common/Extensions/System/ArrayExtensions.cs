using Svelto.Common;
using Svelto.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Extensions.System
{
    public static class ArrayExtensions
    {
        public static NativeDynamicArrayCast<T> ToNativeDynamicArray<T>(this T[] array, Allocator allocator = Allocator.Persistent)
            where T : struct
        {
            NativeDynamicArrayCast<T> native = new NativeDynamicArrayCast<T>((uint)array.Length, allocator);

            for(int i=0; i<array.Length; i++)
            {
                native.Set(i, array[i]);
            }

            return native;
        }
    }
}
