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
    }
}
