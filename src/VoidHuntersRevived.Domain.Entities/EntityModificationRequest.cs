using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Entities.Components;
using System.Runtime.CompilerServices;
using Svelto.ECS;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities
{
    internal unsafe struct EntityModificationRequest
    {
        public readonly EntityModificationType ModificationType;
        public readonly EntityId Id;

        public EntityModificationRequest(
            EntityModificationType modificationType,
            in EntityId id)
        {
            ModificationType = modificationType;
            Id = id;
        }
    }

    internal enum EntityModificationType
    {
        SoftSpawn,
        SoftDespawn,
        RevertSoftDespawn,
        HardDespawn
    }
}
