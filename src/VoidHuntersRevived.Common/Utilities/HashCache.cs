using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Utilities
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
        private readonly HashSet<T> _hashed;
        private Cached _item;

        public HashCache(TimeSpan maximumAge)
        {
            _maximumAge = maximumAge;
            _cached = new Queue<Cached>();
            _hashed = new HashSet<T>();
        }

        public IEnumerable<T> Prune()
        {
            while(_cached.Count > 0 && DateTime.Now - _cached.Peek().CachedAt > _maximumAge)
            {
                _item = _cached.Dequeue();
                if(this.Remove(_item.Value))
                {
                    yield return _item.Value;
                }
            }
        }

        public bool Add(in T item)
        {
            if(_hashed.Add(item))
            {
                _cached.Enqueue(new Cached(in item));
                return true;
            }

            return false;
        }

        public bool Remove(in T item)
        {
            return _hashed.Remove(item);
        }

        public bool Contains(in T item)
        {
            return _hashed.Contains(item);
        }
    }
}
