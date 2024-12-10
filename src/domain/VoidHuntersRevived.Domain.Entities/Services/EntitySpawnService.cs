using Serilog;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Events;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public partial class EntitySpawnService(
        EntityQueryService entityQueryService,
        IEntityTemplateService entityTemplateService,
        IEntityService entityService,
        ILogger logger) : StrategyEngine, IEntitySpawnService, IPrivateEntitySpawnService
    {
        private readonly EntityQueryService _entityQueryService = entityQueryService;
        private readonly IEntityTemplateService _entityTemplateService = entityTemplateService;
        private readonly IEntityService _entityService = entityService;
        private readonly ILogger _logger = logger;

        EntityId IEntitySpawnService.Spawn(VhId sourceId, Key<IEntityTemplate> entityTemplateKey, GlobalEntityId globalId)
        {
            this.Strategy.Publish(NameSpace<EntityService>.Instance.Create(sourceId), new SpawnEntity()
            {
                IsPrivate = false,
                TemplateKey = entityTemplateKey,
                GlobalId = globalId
            });

            return _entityQueryService.GetId(globalId.Value);
        }

        EntityId IEntitySpawnService.Spawn(VhId sourceId, Key<IEntityTemplate> entityTemplateKey, GlobalEntityId globalId, EntityInitializerDelegate initializer)
        {
            this.Strategy.Publish(NameSpace<EntityService>.Instance.Create(sourceId), new SpawnEntity<EntityInitializerDelegate>()
            {
                IsPrivate = false,
                TemplateKey = entityTemplateKey,
                GlobalId = globalId,
                Initializer = initializer
            });

            return _entityQueryService.GetId(globalId.Value);
        }

        void IEntitySpawnService.Despawn(VhId sourceId, GlobalEntityId globalId)
        {
            this.Strategy.Publish(NameSpace<EntityService>.Instance.Create(sourceId), new DespawnEntity()
            {
                IsPrivate = false,
                GlobalId = globalId
            });
        }

        EntityId IPrivateEntitySpawnService.Spawn(VhId sourceId, Key<IEntityTemplate> entityTemplateKey, GlobalEntityId globalId)
        {
            this.Strategy.Publish(NameSpace<EntityService>.Instance.Create(sourceId), new SpawnEntity()
            {
                IsPrivate = true,
                TemplateKey = entityTemplateKey,
                GlobalId = globalId
            });

            return _entityQueryService.GetId(globalId.Value);
        }

        EntityId IPrivateEntitySpawnService.Spawn(VhId sourceId, Key<IEntityTemplate> entityTemplateKey, GlobalEntityId globalId, EntityInitializerDelegate initializer)
        {
            this.Strategy.Publish(NameSpace<EntityService>.Instance.Create(sourceId), new SpawnEntity<EntityInitializerDelegate>()
            {
                IsPrivate = true,
                TemplateKey = entityTemplateKey,
                GlobalId = globalId,
                Initializer = initializer
            });

            return _entityQueryService.GetId(globalId.Value);
        }

        void IPrivateEntitySpawnService.Despawn(VhId sourceId, GlobalEntityId globalId)
        {
            this.Strategy.Publish(NameSpace<EntityService>.Instance.Create(sourceId), new DespawnEntity()
            {
                IsPrivate = true,
                GlobalId = globalId
            });
        }
    }
}
