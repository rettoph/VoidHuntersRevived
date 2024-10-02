using System.Runtime.InteropServices;

namespace VoidHuntersRevived.Common.Utilities
{
    public class HashCache<T>(TimeSpan maximumAge)
        where T : struct
    {
        private struct Cached(in T value)
        {
            public readonly T Value = value;
            public readonly DateTime CachedAt = DateTime.Now;
        }

        private readonly TimeSpan _maximumAge = maximumAge;
        private readonly Queue<Cached> _cached = new Queue<Cached>();
        private readonly Dictionary<T, int> _count = new Dictionary<T, int>();
        private Cached _item;

        public IEnumerable<T> Prune()
        {
            while (_cached.Count > 0 && DateTime.Now - _cached.Peek().CachedAt > _maximumAge)
            {
                _item = _cached.Dequeue();
                if (_count.Remove(_item.Value))
                {
                    yield return _item.Value;
                }
            }
        }

        public int Add(in T item)
        {
            return ++this.Count(in item);
        }

        public int Remove(in T item)
        {
            return --this.Count(in item);
        }

        public bool Any(in T item)
        {
            return this.Count(in item) != 0;
        }

        public ref int Count(in T item)
        {
            ref int count = ref CollectionsMarshal.GetValueRefOrAddDefault(_count, item, out bool exists);
            if (!exists)
            {
                _cached.Enqueue(new Cached(in item));
                count = 0;
            }

            return ref count;
        }
    }
}
