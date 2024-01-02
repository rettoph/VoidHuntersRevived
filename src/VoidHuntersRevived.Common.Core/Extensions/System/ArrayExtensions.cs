using Svelto.Common;
using Svelto.DataStructures;

namespace VoidHuntersRevived.Common.Core.Extensions.System
{
    public static class ArrayExtensions
    {
        public static NativeDynamicArrayCast<T> ToNativeDynamicArray<T>(this T[] array)
            where T : struct
        {
            NativeDynamicArrayCast<T> native = new NativeDynamicArrayCast<T>((uint)array.Length, Allocator.Persistent);

            for (int i = 0; i < array.Length; i++)
            {
                native.Set(i, array[i]);
            }

            return native;
        }
    }
}
