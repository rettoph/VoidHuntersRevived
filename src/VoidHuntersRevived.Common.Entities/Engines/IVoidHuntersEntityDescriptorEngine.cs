using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Options;
using VoidHuntersRevived.Common.Entities.Serialization;

namespace VoidHuntersRevived.Common.Entities.Engines
{
    internal interface IVoidHuntersEntityDescriptorEngine
    {
        VoidHuntersEntityDescriptor Descriptor { get; }

        EntityInitializer HardSpawn(in VhId sourceEventId, in VhId vhid, in Id<ITeam> teamId, out EntityId id);
        void SoftSpawn(in VhId sourceEventId, in EntityId id, in GroupIndex groupIndex, ref EntityStatus status);

        void SoftDespawn(in VhId sourceEventId, in EntityId id, in GroupIndex groupIndex, ref EntityStatus status);
        void RevertSoftDespawn(in VhId sourceEventId, in EntityId id, in GroupIndex groupIndex, ref EntityStatus status);

        void HardDespawn(in VhId sourceEventId, in EntityId id, in GroupIndex groupIndex, ref EntityStatus status);

        void Serialize(EntityWriter writer, in GroupIndex groupIndex, in SerializationOptions options);
        void Deserialize(in VhId sourceId, in DeserializationOptions options, EntityReader reader, ref EntityInitializer initializer, in EntityId id);
    }
}
