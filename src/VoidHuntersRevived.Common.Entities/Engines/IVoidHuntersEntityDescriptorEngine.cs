using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Options;
using VoidHuntersRevived.Common.Entities.Serialization;

namespace VoidHuntersRevived.Common.Entities.Engines
{
    internal interface IVoidHuntersEntityDescriptorEngine
    {
        VoidHuntersEntityDescriptor Descriptor { get; }

        EntityInitializer HardSpawn(in VhId vhid, in Id<ITeam> teamId, out EntityId id);
        void SoftSpawn(in EntityId id, in GroupIndex groupIndex, ref EntityStatus status);

        void SoftDespawn(in EntityId id, in GroupIndex groupIndex, ref EntityStatus status);
        void RevertSoftDespawn(in EntityId id, in GroupIndex groupIndex, ref EntityStatus status);

        void HardDespawn(in EntityId id, in GroupIndex groupIndex, ref EntityStatus status);

        void Serialize(EntityWriter writer, in GroupIndex groupIndex, in SerializationOptions options);
        void Deserialize(in DeserializationOptions options, EntityReader reader, ref EntityInitializer initializer, in EntityId id);
    }
}
