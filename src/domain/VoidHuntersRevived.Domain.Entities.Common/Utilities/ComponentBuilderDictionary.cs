using Guppy.Core.Common;
using Guppy.Core.Common.Extensions.System;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Interfaces;

namespace VoidHuntersRevived.Domain.Entities.Common.Utilities
{
    public class ComponentBuilderDictionary
    {
        private readonly Dictionary<Type, IEntityComponent> _values;
        private readonly Dictionary<Type, IComponentBuilder> _builders;

        public IEnumerable<Type> Keys => this._values.Keys;

        public ComponentBuilderDictionary()
        {
            this._values = [];
            this._builders = [];
        }

        public ComponentBuilderDictionary(IEnumerable<ComponentBuilderDictionary> dictionaries) : this()
        {
            foreach (ComponentBuilderDictionary dictionary in dictionaries)
            {
                foreach (Type key in dictionary.Keys)
                {
                    if (dictionary._values[key] == default)
                    {
                        continue;
                    }

                    this._values[key] = dictionary._values[key];
                    this._builders[key] = dictionary._builders[key];
                }
            }
        }

        public void Set(IEntityComponent component)
        {
            Type componentType = component.GetType();

            ThrowIf.Type.IsNotAssignableFrom<IEntityComponent>(componentType);
            ThrowIf.Type.IsNotUnmanagedStruct(componentType);

            Type componentBuilderType = typeof(ComponentBuilder<>).MakeGenericType(componentType);
            if (componentType.ImplementsGenericTypeDefinition(typeof(ICloneableComponent<>)))
            {
                componentBuilderType = typeof(CloneableComponentBuilder<>).MakeGenericType(componentType);
            }

            IComponentBuilder builder = (IComponentBuilder)(Activator.CreateInstance(componentBuilderType, component) ?? throw new NotImplementedException());

            this._values[componentType] = component;
            this._builders[componentType] = builder;
        }

        public bool TryGet<T>(out T component)
            where T : unmanaged, IEntityComponent
        {
            if (this._values.TryGetValue(typeof(T), out IEntityComponent? uncasted) == false)
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
            if (this._values.TryGetValue(typeof(T), out IEntityComponent? uncasted) == false)
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
            where T : unmanaged, IEntityComponent => this._builders.ContainsKey(typeof(T));

        public bool Has(Type type) => this._builders.ContainsKey(type);

        public IComponentBuilder[] ToArray() => [.. this._builders.Values];

        public Dictionary<Type, IEntityComponent> ToComponentDictionary() => this._values;

        public static implicit operator IComponentBuilder[](ComponentBuilderDictionary dictionary)
        {
            return dictionary.ToArray();
        }
    }
}