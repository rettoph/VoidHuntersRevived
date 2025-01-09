using System.Runtime.InteropServices;

namespace VoidHuntersRevived.Common.Utilities
{
    public class HashCache<T>(TimeSpan maximumAge)
        where T : struct
    {
        private readonly struct Cached(in T value)
        {
            public readonly T Value = value;
            public readonly DateTime CachedAt = DateTime.Now;
        }

        private readonly TimeSpan _maximumAge = maximumAge;
        private readonly Queue<Cached> _cached = new();
        private readonly Dictionary<T, int> _count = [];
        private Cached _item;

        public IEnumerable<T> Prune()
        {
            while (this._cached.Count > 0 && DateTime.Now - this._cached.Peek().CachedAt > this._maximumAge)
            {
                this._item = this._cached.Dequeue();
                if (this._count.Remove(this._item.Value))
                {
                    yield return this._item.Value;
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
            ref int count = ref CollectionsMarshal.GetValueRefOrAddDefault(this._count, item, out bool exists);
            if (!exists)
            {
                this._cached.Enqueue(new Cached(in item));
                count = 0;
            }

            return ref count;
        }
    }
}