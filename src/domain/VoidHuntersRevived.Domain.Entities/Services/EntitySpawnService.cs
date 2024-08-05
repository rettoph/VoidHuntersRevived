using Serilog;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Events;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public partial class EntitySpawnService : StrategyEngine, IEntitySpawnService, IPrivateEntitySpawnService
    {
        private readonly EntityQueryService _entityQueryService;
        private readonly IEntityService _entityService;
        private readonly ILogger _logger;

        public EntitySpawnService(
            EntityQueryService entityQueryingService,
            IEntityService entityService,
            ILogger logger)
        {
            _entityQueryService = entityQueryingService;
            _entityService = entityService;
            _logger = logger;
        }

        EntityId IEntitySpawnService.Spawn(VhId sourceId, IEntityType type, VhId vhid)
        {
            this.Strategy.Publish(NameSpace<EntityService>.Instance.Create(sourceId), new SpawnEntity()
            {
                IsPrivate = false,
                Type = type,
                VhId = vhid
            });

            return _entityQueryService.GetId(vhid);
        }

        EntityId IEntitySpawnService.Spawn(VhId sourceId, IEntityType type, VhId vhid, EntityInitializerDelegate initializer)
        {
            this.Strategy.Publish(NameSpace<EntityService>.Instance.Create(sourceId), new SpawnEntity<EntityInitializerDelegate>()
            {
                IsPrivate = false,
                Type = type,
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

        EntityId IPrivateEntitySpawnService.Spawn(VhId sourceId, IEntityType type, VhId vhid)
        {
            this.Strategy.Publish(NameSpace<EntityService>.Instance.Create(sourceId), new SpawnEntity()
            {
                IsPrivate = true,
                Type = type,
                VhId = vhid
            });

            return _entityQueryService.GetId(vhid);
        }

        EntityId IPrivateEntitySpawnService.Spawn(VhId sourceId, IEntityType type, VhId vhid, EntityInitializerDelegate initializer)
        {
            this.Strategy.Publish(NameSpace<EntityService>.Instance.Create(sourceId), new SpawnEntity<EntityInitializerDelegate>()
            {
                IsPrivate = true,
                Type = type,
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
