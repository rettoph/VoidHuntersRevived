using Guppy.Core.Common.Collections;
using System.Diagnostics;

namespace VoidHuntersRevived.Common
{
    [DebuggerDisplay("Type = {Type.Name}, Name = {Name}")]
    public struct Key<T>(VhId id)
    {
        private static readonly Map<VhId, string> _map = new Map<VhId, string>();

        public readonly VhId Id = id;
        public string Name => _map[this.Id];
        public Type Type => typeof(T);

        public static Key<T> GetByName(string name)
        {
            if (_map.TryGet(name, out VhId id) == true)
            {
                return new Key<T>(id);
            }

            id = NameSpace<T>.Instance.Create(name);
            if (_map.TryGet(id, out _) == true)
            {
                _map[id] = name;
                return new Key<T>(id);
            }

            if (_map.TryAdd(id, name) == true)
            {
                return new Key<T>(id);
            }

            throw new NotImplementedException();
        }

        public static Key<T> GetById(VhId id)
        {
            if (_map.TryGet(id, out _) == true)
            {
                return new Key<T>(id);
            }

            if (_map.TryAdd(id, id.ToString()) == true)
            {
                return new Key<T>(id);
            }

            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"Key<{this.Type.Name}>('{this.Name}')";
        }

        public static bool operator ==(Key<T> a, Key<T> b)
        {
            return a.Id == b.Id;
        }

        public static bool operator !=(Key<T> a, Key<T> b)
        {
            return a.Id != b.Id;
        }

        public override bool Equals(object? obj)
        {
            return obj is Key<T> key &&
                   Id.Equals(key.Id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
