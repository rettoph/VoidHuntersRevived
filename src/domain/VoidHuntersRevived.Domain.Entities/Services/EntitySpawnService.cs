using Guppy.Core.Common.Attributes;
using Serilog;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Events;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    [Sequence<EngineSequence>(EngineSequence.Group03)]
    public partial class EntitySpawnService : StrategyEngine, IEntitySpawnService, IPrivateEntitySpawnService
    {
        private readonly EntityQueryService _entityQueryService;
        private readonly IEntityTypeProviderService _entityTypeProviderService;
        private readonly IEntityService _entityService;
        private readonly ILogger _logger;

        public EntitySpawnService(
            EntityQueryService entityQueryingService,
            IEntityTypeProviderService entityTypeProviderService,
            IEntityService entityService,
            ILogger logger)
        {
            _entityQueryService = entityQueryingService;
            _entityTypeProviderService = entityTypeProviderService;
            _entityService = entityService;
            _logger = logger;
        }

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
