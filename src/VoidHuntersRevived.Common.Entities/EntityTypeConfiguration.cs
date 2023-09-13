using Guppy.Common.Helpers;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.ECS.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Entities
{
    internal sealed class EntityTypeConfiguration<TDescriptor> : IEntityTypeConfiguration
        where TDescriptor : VoidHuntersEntityDescriptor, new()
    {
        private EntityInitializerDelegate? _initializer;
        private Action? _disposer;

        public IEntityType<TDescriptor> Type { get; }
        IEntityType IEntityTypeConfiguration.Type => Type;

        public EntityTypeConfiguration(IEntityType<TDescriptor> type)
        {
            Type = type;
        }

        public IEntityTypeConfiguration InitializeComponent<T>(IEntityTypeComponentValue<T> componentInitializer)
            where T : unmanaged, IEntityComponent
        {
            if(!this.Type.Descriptor.ComponentManagers.Any(x => x.Type == typeof(T)))
            {
                throw new Exception();
            }

            _initializer += (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
            {
                initializer.Init<T>(componentInitializer.GetInstance(id));
            };

            _disposer += componentInitializer.Dispose;

            return this;
        }

        public void Initialize(IEntityService entities, ref EntityInitializer initializer, in EntityId id)
        {
            initializer.Init<EntityTypeId>(this.Type.Id);
            _initializer?.Invoke(entities, ref initializer, in id);
        }

        public void Dispose()
        {
            _disposer?.Invoke();
        }
    }
}
