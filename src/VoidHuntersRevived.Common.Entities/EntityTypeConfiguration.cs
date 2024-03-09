using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Entities
{
    internal sealed class EntityTypeConfiguration<TDescriptor> : IEntityTypeConfiguration
        where TDescriptor : VoidHuntersEntityDescriptor, new()
    {
        private StaticEntityInitializerDelegate? _staticInitializer;
        private InstanceEntityInitializerDelegate? _instanceInitializer;

        private Action? _disposer;

        public IEntityType<TDescriptor> Type { get; }
        IEntityType IEntityTypeConfiguration.Type => Type;

        public EntityTypeConfiguration(IEntityType<TDescriptor> type)
        {
            Type = type;
        }

        public void Dispose()
        {
            _disposer?.Invoke();
        }

        public IEntityTypeConfiguration InitializeInstanceComponent<T>(IEntityTypeComponentInitializer<T> componentInitializer)
            where T : unmanaged, IEntityComponent
        {
            if (!this.Type.Descriptor.componentsToBuild.Any(x => x.GetEntityComponentType() == typeof(T)))
            {
                throw new Exception();
            }

            _instanceInitializer += (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
            {
                initializer.Init<T>(componentInitializer.GetInstance(id));
            };

            _disposer += componentInitializer.Dispose;

            return this;
        }

        public void InitializeInstance(IEntityService entities, ref EntityInitializer initializer, in EntityId id)
        {
            initializer.Init<Id<IEntityType>>(this.Type.Id);
            _instanceInitializer?.Invoke(entities, ref initializer, in id);

            this.Type.Descriptor.PostInitialize(entities, ref initializer, in id);
        }

        public IEntityTypeConfiguration InitializeStaticComponent<T>(T instance)
            where T : unmanaged, IEntityComponent
        {
            if (!this.Type.Descriptor.StaticDescriptor.componentsToBuild.Any(x => x.GetEntityComponentType() == typeof(T)))
            {
                throw new Exception();
            }

            _staticInitializer += (ref EntityInitializer initializer) =>
            {
                initializer.Init<T>(instance);
            };

            if (instance is IDisposable disposable)
            {
                _disposer += disposable.Dispose;
            }


            return this;
        }

        public void InitializeStatic(ref EntityInitializer initializer)
        {
            _staticInitializer?.Invoke(ref initializer);
        }
    }
}
