using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Common.Utilities;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    /// <summary>
    /// Represents the conglomerate of an entity type functions, including spawning, despawning, initialization, and serialization.
    /// </summary>
    public interface IEntityTemplate
    {
        Key<IEntityTemplate> Key { get; }
        ComponentBuilderDictionary Components { get; }

        void Initialize(
            EntitiesDB entitiesDB,
            IEngineService engineService,
            IComponentSerializerService componentSerializerService);

        EntityInitializer HardSpawnInstanceEntity(in VhId sourceEventId, in EntityGlobalId globalId, out EntityLocalId localId);
        void SoftSpawnInstanceEntity(in VhId sourceEventId, in EntityGlobalId globalId, in EntityLocalId localId, in GroupIndex groupIndex, ref EntityStatus status);

        void SoftDespawnInstanceEntity(in VhId sourceEventId, in EntityGlobalId globalId, in EntityLocalId localId, in GroupIndex groupIndex, ref EntityStatus status);
        void HardDespawnInstanceEntity(in VhId sourceEventId, in EntityGlobalId globalId, in EntityLocalId localId, in GroupIndex groupIndex, ref EntityStatus status);

        void SerializeInstanceEntity(ref EntityWriter writer, in EntityId id, in GroupIndex groupIndex, in SerializationOptions options);
        void DeserializeInstanceEntity(in VhId sourceId, in DeserializationOptions options, ref EntityReader reader, ref EntityInitializer initializer, in EntityId id);
    }
}
