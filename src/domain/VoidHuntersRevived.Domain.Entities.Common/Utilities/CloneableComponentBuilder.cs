using Svelto.ECS;
using Svelto.ECS.Internal;
using VoidHuntersRevived.Domain.Entities.Common.Interfaces;

namespace VoidHuntersRevived.Domain.Entities.Common.Utilities
{
    internal class CloneableComponentBuilder<T>(T instance) : IComponentBuilder, IDisposable
        where T : unmanaged, ICloneableComponent<T>
    {
        private readonly ComponentBuilder<T> _builder = new ComponentBuilder<T>(instance);
        private readonly T _instance = instance;

        bool IComponentBuilder.isUnmanaged => true;
        ComponentID IComponentBuilder.getComponentID => ComponentTypeID<T>.id;

        void IComponentBuilder.BuildEntityAndAddToList(ITypeSafeDictionary dictionary, EGID egid, IEnumerable<object> implementors)
        {
            var castedDic = dictionary as ITypeSafeDictionary<T>;
            castedDic!.Add(egid.entityID, _instance.Clone());
        }

        ITypeSafeDictionary IComponentBuilder.CreateDictionary(uint size)
        {
            return _builder.CreateDictionary(size);
        }

        Type IComponentBuilder.GetEntityComponentType()
        {
            return typeof(T);
        }

        void IComponentBuilder.Preallocate(ITypeSafeDictionary dictionary, uint size)
        {
            dictionary.EnsureCapacity(size);
        }

        void IDisposable.Dispose()
        {
            if (_instance is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
