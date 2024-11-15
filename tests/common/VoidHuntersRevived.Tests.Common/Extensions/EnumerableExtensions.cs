using Guppy.Tests.Common.Mocks;

namespace VoidHuntersRevived.Tests.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static MockFiltered<T> ToFiltered<T>(this IEnumerable<T> items)
            where T : class
        {
            return new MockFiltered<T>(items);
        }

        public static byte[] ToByteArray<T>(this IEnumerable<T> items, Func<T, byte[]> converter)
        {
            return items.SelectMany(converter).ToArray();
        }
    }
}
