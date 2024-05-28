using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Common.Providers
{
    /// <summary>
    /// Represents the conglomerate of an entity type functions, including spawning, despawning, initialization, and serialization.
    /// </summary>
    public interface IEntityTypeProvider : IDisposable
    {
        public IEntityType Type { get; }

        EntityInitializer HardSpawnInstance(in VhId sourceEventId, in VhId vhid, out EntityId id);
        void SoftSpawnInstance(in VhId sourceEventId, in EntityId id, in GroupIndex groupIndex, ref EntityStatus status);

        void SoftDespawnInstance(in VhId sourceEventId, in EntityId id, in GroupIndex groupIndex, ref EntityStatus status);
        void HardDespawnInstance(in VhId sourceEventId, in EntityId id, in GroupIndex groupIndex, ref EntityStatus status);

        void SerializeInstance(EntityWriter writer, in GroupIndex groupIndex, in SerializationOptions options);
        void DeserializeInstance(in VhId sourceId, in DeserializationOptions options, EntityReader reader, ref EntityInitializer initializer, in EntityId id);

        void InitializeType(IEntityService entities, IEntityType type, in EntityId typeEntityId, ref EntityInitializer entityInitializer);
    }
}
