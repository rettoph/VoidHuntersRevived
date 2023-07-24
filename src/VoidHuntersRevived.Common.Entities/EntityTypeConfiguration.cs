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

        public IEntityType<TDescriptor> Type { get; }
        IEntityType IEntityTypeConfiguration.Type => Type;

        public EntityTypeConfiguration(IEntityType<TDescriptor> type)
        {
            Type = type;
        }

        public IEntityTypeConfiguration HasInitializer(EntityInitializerDelegate initializer)
        {
            _initializer += initializer;

            return this;
        }

        public void Initialize(IEntityService entities, ref EntityInitializer initializer, in EntityId id)
        {
            _initializer?.Invoke(entities, ref initializer, in id);
        }
    }
}
