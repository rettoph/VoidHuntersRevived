using Guppy.Core.Common;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Utilities
{
    public class ComponentBuilderDictionary
    {
        private readonly Dictionary<Type, IEntityComponent> _values;
        private readonly Dictionary<Type, IComponentBuilder> _builders;

        public IEnumerable<Type> Keys => _values.Keys;

        public ComponentBuilderDictionary()
        {
            _values = new Dictionary<Type, IEntityComponent>();
            _builders = new Dictionary<Type, IComponentBuilder>();
        }

        public ComponentBuilderDictionary(IEnumerable<ComponentBuilderDictionary> dictionaries) : this()
        {
            foreach (ComponentBuilderDictionary dictionary in dictionaries)
            {
                this._values.Merge(dictionary._values);
                this._builders.Merge(dictionary._builders);
            }
        }

        public void Set<T>(T component)
            where T : unmanaged, IEntityComponent
        {
            _values[typeof(T)] = component;
            _builders[typeof(T)] = new ComponentBuilder<T>(component);
        }

        public void Set(IEntityComponent component)
        {
            Type componentType = component.GetType();

            ThrowIf.Type.IsNotAssignableFrom<IEntityComponent>(componentType);
            ThrowIf.Type.IsNotUnmanagedStruct(componentType);

            Type componentBuilderType = typeof(ComponentBuilder<>).MakeGenericType(componentType);
            IComponentBuilder builder = (IComponentBuilder)(Activator.CreateInstance(componentBuilderType, component) ?? throw new NotImplementedException());

            _values[componentType] = component;
            _builders[componentType] = builder;
        }

        public bool TryGet<T>(out T component)
            where T : unmanaged, IEntityComponent
        {
            if (_values.TryGetValue(typeof(T), out IEntityComponent? uncasted) == false)
            {
                component = default;
                return false;
            }

            if (uncasted is not T casted)
            {
                component = default;
                return false;
            }

            component = casted;
            return true;
        }

        public T Get<T>()
        {
            if (_values.TryGetValue(typeof(T), out IEntityComponent? uncasted) == false)
            {
                throw new KeyNotFoundException();
            }

            if (uncasted is not T casted)
            {
                throw new InvalidCastException();
            }

            return casted;
        }

        public bool Has<T>()
            where T : unmanaged, IEntityComponent
        {
            return _builders.ContainsKey(typeof(T));
        }

        public IComponentBuilder[] ToArray()
        {
            return _builders.Values.ToArray();
        }

        public static implicit operator IComponentBuilder[](ComponentBuilderDictionary dictionary)
        {
            return dictionary.ToArray();
        }
    }
}
