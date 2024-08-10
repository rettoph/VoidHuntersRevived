using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace VoidHuntersRevived.Common
{
    public abstract class Key : IKey
    {
        public string Name { get; }

        public VhId Id { get; }

        public abstract Type Type { get; }

        internal Key(VhId id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        private static Dictionary<VhId, IKey> _keys = new Dictionary<VhId, IKey>();

        public static IKey<T> GetById<T>(VhId id)
        {
            if (_keys.TryGetValue(id, out IKey? key) == false)
            {
                throw new KeyNotFoundException();
            }

            if (key is not IKey<T> casted)
            {
                throw new ArgumentException();
            }

            return casted;
        }

        public static IKey<T> GetByName<T>(string name)
        {
            VhId id = NameSpace<Key>.Instance.Create(name);
            ref IKey? key = ref CollectionsMarshal.GetValueRefOrAddDefault(_keys, id, out bool exists);

            if (exists == false)
            {
                IKey<T> newKey = new Key<T>(id, name);
                key = newKey;

                return newKey;
            }

            if (key is IKey<T> casted)
            {
                return casted;
            }

            throw new NotImplementedException();

            // Do we want to change the key ref at runtime?
            // i dont think so...
            if (typeof(T).IsAssignableTo(key!.Type))
            {
                IKey<T> reKey = new Key<T>(id, name);
                key = reKey;

                return reKey;
            }

            throw new NotImplementedException();
        }

        private static MethodInfo _getByNameMethodInfo = typeof(Key).GetMethod(nameof(GetByName), BindingFlags.Public | BindingFlags.Static, [typeof(string)]) ?? throw new NotImplementedException();
        public static IKey GetByName(string name, Type type)
        {
            object? key = _getByNameMethodInfo.MakeGenericMethod(type).Invoke(null, [name]);

            if (key is not IKey casted)
            {
                throw new InvalidOperationException();
            }

            return casted;
        }

        public bool Equals(IKey? other)
        {
            return object.ReferenceEquals(this, other);

        }
    }

    [DebuggerDisplay("Type = {Type.Name}, Name = {Name}")]
    internal class Key<T> : Key, IKey<T>
    {
        public override Type Type => typeof(T);

        public Key(VhId id, string name) : base(id, name)
        {
        }

        public override string ToString()
        {
            return $"Key<{this.Type.Name}>('{this.Name}')";
        }
    }
}
