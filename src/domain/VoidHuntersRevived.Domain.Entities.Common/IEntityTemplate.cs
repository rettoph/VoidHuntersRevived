using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    /// <summary>
    /// Represents the conglomerate of an entity type functions, including spawning, despawning, initialization, and serialization.
    /// </summary>
    public interface IEntityTemplate
    {
        Key<IEntityTemplate> Key { get; }
        ComponentBuilderDictionary Components { get; }

        EntityInitializer HardSpawnEntity(in VhId sourceEventId, in EntityGlobalId globalId, out EntityLocalId localId);
        void SoftSpawnEntity(in VhId sourceEventId, in Entity entity, ref EntityStatus status);

        void SoftDespawnEntity(in VhId sourceEventId, in Entity entity, ref EntityStatus status);
        void HardDespawnEntity(in VhId sourceEventId, in Entity entity, ref EntityStatus status);

        void SerializeEntity(ref EntityWriter writer, in Entity entity, in SerializationOptions options);
        void DeserializeEntity(in VhId sourceId, in DeserializationOptions options, ref EntityReader reader, in InitializingEntity entity);
    }
}