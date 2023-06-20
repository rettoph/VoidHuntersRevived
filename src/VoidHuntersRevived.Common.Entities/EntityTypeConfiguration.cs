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
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Domain.Common.Components;

namespace VoidHuntersRevived.Common.Entities
{
    internal sealed class EntityTypeConfiguration<TDescriptor> : IEntityTypeConfiguration
        where TDescriptor : IEntityDescriptor, new()
    {
        private EntityInitializerDelegate? _initializer;

        public EntityType<TDescriptor> Type { get; }
        EntityType IEntityTypeConfiguration.Type => Type;

        public EntityTypeConfiguration(EntityType<TDescriptor> type)
        {
            Type = type;
        }

        public IEntityTypeConfiguration HasInitializer(EntityInitializerDelegate initializer)
        {
            _initializer += initializer;

            return this;
        }

        public void Initialize(ref EntityInitializer initializer)
        {
            _initializer?.Invoke(ref initializer);
        }
    }
}
