using Svelto.Common;
using Svelto.DataStructures;

namespace VoidHuntersRevived.Common.Extensions.Svelto
{
    public static class NativeDynamicArrayCastExtensions
    {
        public static IEnumerable<TOut> Select<TIn, TOut>(this NativeDynamicArrayCast<TIn> native, Func<TIn, TOut> selector)
            where TIn : struct
        {
            for (int i = 0; i < native.count; i++)
            {
                yield return selector(native[i]);
            }
        }

        public static NativeDynamicArrayCast<T> Clone<T>(this NativeDynamicArrayCast<T> source, Allocator allocator)
            where T : struct
        {
            NativeDynamicArrayCast<T> clone = new NativeDynamicArrayCast<T>((uint)source.count, allocator);
            for (int i = 0; i < source.count; i++)
            {
                clone.Set(i, source[i]);
            }

            return clone;
        }

        public static NativeDynamicArrayCast<T> Clone<T>(this NativeDynamicArrayCast<T> source, Allocator allocator, Func<T, T> cloner)
            where T : struct
        {
            NativeDynamicArrayCast<T> clone = new NativeDynamicArrayCast<T>((uint)source.count, allocator);
            for (int i = 0; i < source.count; i++)
            {
                clone.Set(i, cloner(source[i]));
            }

            return clone;
        }
    }
}
