using System.Runtime.InteropServices;

namespace VoidHuntersRevived.Common.Core.Utilities
{
    public class HashCache<T>
        where T : struct
    {
        private struct Cached
        {
            public readonly T Value;
            public readonly DateTime CachedAt;

            public Cached(in T value)
            {
                Value = value;
                CachedAt = DateTime.Now;
            }
        }

        private readonly TimeSpan _maximumAge;
        private readonly Queue<Cached> _cached;
        private readonly Dictionary<T, int> _count;
        private Cached _item;

        public HashCache(TimeSpan maximumAge)
        {
            _maximumAge = maximumAge;
            _cached = new Queue<Cached>();
            _count = new Dictionary<T, int>();
        }

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
