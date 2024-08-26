using Guppy.Core.Common;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Initializers;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Common.Utilities;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Common.Providers
{
    /// <summary>
    /// Represents the conglomerate of an entity type functions, including spawning, despawning, initialization, and serialization.
    /// </summary>
    public interface IEntityTypeProvider : IDisposable
    {
        IEntityType Type { get; }
        IEntityType[] ImplementedTypes { get; }

        ComponentBuilderDictionary Components { get; }
        EntityInitializerDelegate? Initializer { get; set; }
        DisposeEntityInitializerDelegate? Disposer { get; set; }

        void Initialize(
            EntitiesDB entitiesDB,
            IEngineService engineService,
            IComponentSerializerService componentSerializerService,
            IFiltered<IEntityTypeProviderInitializer> entityTypeProviderInitializers);

        #region Instance Entity Methods
        EntityInitializer HardSpawnInstanceEntity(in VhId sourceEventId, in VhId vhid, out EntityId id);
        void SoftSpawnInstanceEntity(in VhId sourceEventId, in EntityId id, in GroupIndex groupIndex, ref EntityStatus status);

        void SoftDespawnInstanceEntity(in VhId sourceEventId, in EntityId id, in GroupIndex groupIndex, ref EntityStatus status);
        void HardDespawnInstanceEntity(in VhId sourceEventId, in EntityId id, in GroupIndex groupIndex, ref EntityStatus status);

        void SerializeInstanceEntity(EntityWriter writer, in EntityId id, in GroupIndex groupIndex, in SerializationOptions options);
        void DeserializeInstanceEntity(in VhId sourceId, in DeserializationOptions options, EntityReader reader, ref EntityInitializer initializer, in EntityId id);
        #endregion

        IEnumerable<Type> GetAllDistinctComponentTypes();

        bool Implements(IKey<IEntityType> key);
    }
}
