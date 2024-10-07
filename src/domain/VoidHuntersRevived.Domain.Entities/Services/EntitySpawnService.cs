using Guppy.Core.Common.Attributes;
using Serilog;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Events;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public partial class EntitySpawnService(
        EntityQueryService entityQueryingService,
        IEntityTypeProviderService entityTypeProviderService,
        IEntityService entityService,
        ILogger logger) : StrategyEngine, IEntitySpawnService, IPrivateEntitySpawnService
    {
        private readonly EntityQueryService _entityQueryService = entityQueryingService;
        private readonly IEntityTypeProviderService _entityTypeProviderService = entityTypeProviderService;
        private readonly IEntityService _entityService = entityService;
        private readonly ILogger _logger = logger;

        EntityId IEntitySpawnService.Spawn(VhId sourceId, Key<IEntityType> entityTypeKey, VhId vhid)
        {
            this.Strategy.Publish(NameSpace<EntityService>.Instance.Create(sourceId), new SpawnEntity()
            {
                IsPrivate = false,
                TypeKey = entityTypeKey,
                VhId = vhid
            });

            return _entityQueryService.GetId(vhid);
        }

        EntityId IEntitySpawnService.Spawn(VhId sourceId, Key<IEntityType> entityTypeKey, VhId vhid, EntityInitializerDelegate initializer)
        {
            this.Strategy.Publish(NameSpace<EntityService>.Instance.Create(sourceId), new SpawnEntity<EntityInitializerDelegate>()
            {
                IsPrivate = false,
                TypeKey = entityTypeKey,
                VhId = vhid,
                Initializer = initializer
            });

            return _entityQueryService.GetId(vhid);
        }

        void IEntitySpawnService.Despawn(VhId sourceId, VhId vhid)
        {
            this.Strategy.Publish(NameSpace<EntityService>.Instance.Create(sourceId), new DespawnEntity()
            {
                IsPrivate = false,
                VhId = vhid
            });
        }

        EntityId IPrivateEntitySpawnService.Spawn(VhId sourceId, Key<IEntityType> entityTypeKey, VhId vhid)
        {
            this.Strategy.Publish(NameSpace<EntityService>.Instance.Create(sourceId), new SpawnEntity()
            {
                IsPrivate = true,
                TypeKey = entityTypeKey,
                VhId = vhid
            });

            return _entityQueryService.GetId(vhid);
        }

        EntityId IPrivateEntitySpawnService.Spawn(VhId sourceId, Key<IEntityType> entityTypeKey, VhId vhid, EntityInitializerDelegate initializer)
        {
            this.Strategy.Publish(NameSpace<EntityService>.Instance.Create(sourceId), new SpawnEntity<EntityInitializerDelegate>()
            {
                IsPrivate = true,
                TypeKey = entityTypeKey,
                VhId = vhid,
                Initializer = initializer
            });

            return _entityQueryService.GetId(vhid);
        }

        void IPrivateEntitySpawnService.Despawn(VhId sourceId, VhId vhid)
        {
            this.Strategy.Publish(NameSpace<EntityService>.Instance.Create(sourceId), new DespawnEntity()
            {
                IsPrivate = true,
                VhId = vhid
            });
        }
    }
}
