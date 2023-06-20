using Svelto.ECS;
using Svelto.ECS.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Domain.Common.Components;

namespace VoidHuntersRevived.Domain.Entities.Abstractions
{
    [HashName("VHR.EntityDescriptor")]
    internal sealed class EntityDescriptor : IEntityDescriptor
    {
        private readonly IComponentBuilder[] _componentsToBuild = new[]
        {
            new SerializableComponentBuilder<SerializationType, EntityVhId>(
                ((int)SerializationType.Storage, new DefaultSerializer<EntityVhId>()),
                ((int)SerializationType.Backup, new DefaultSerializer<EntityVhId>()),
                ((int)SerializationType.Clone, new DontSerialize<EntityVhId>())
            ),
        };

        public IComponentBuilder[] componentsToBuild => _componentsToBuild;

        public EntityDescriptor()
        {
        }
    }
}
