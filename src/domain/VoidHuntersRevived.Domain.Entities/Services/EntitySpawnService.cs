using Guppy.Core.Logging.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Events;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public partial class EntitySpawnService(
        EntityQueryService entityQueryService,
        IStrategy strategy,
        IEntityTemplateService entityTemplateService,
        IEntityService entityService,
        ILogger logger) : IEntitySpawnService, IPrivateEntitySpawnService
    {
        private readonly IStrategy _strategy = strategy;
        private readonly EntityQueryService _entityQueryService = entityQueryService;
        private readonly IEntityTemplateService _entityTemplateService = entityTemplateService;
        private readonly IEntityService _entityService = entityService;
        private readonly ILogger _logger = logger;

        EntityLocalId IEntitySpawnService.Spawn(VhId sourceId, Key<IEntityTemplate> entityTemplateKey, EntityGlobalId globalId)
        {
            this._strategy.Publish(NameSpace<EntitySpawnService>.Instance.Create(sourceId), new SpawnEntity()
            {
                IsPrivate = false,
                TemplateKey = entityTemplateKey,
                GlobalId = globalId
            });

            return this._entityQueryService.GetLocalId(globalId);
        }

        EntityLocalId IEntitySpawnService.Spawn(VhId sourceId, Key<IEntityTemplate> entityTemplateKey, EntityGlobalId globalId, EntityInitializerDelegate initializer)
        {
            this._strategy.Publish(NameSpace<EntitySpawnService>.Instance.Create(sourceId), new SpawnEntity<EntityInitializerDelegate>()
            {
                IsPrivate = false,
                TemplateKey = entityTemplateKey,
                GlobalId = globalId,
                Initializer = initializer
            });

            return this._entityQueryService.GetLocalId(globalId);
        }

        void IEntitySpawnService.Despawn(VhId sourceId, EntityGlobalId globalId)
        {
            this._strategy.Publish(NameSpace<EntitySpawnService>.Instance.Create(sourceId), new DespawnEntity()
            {
                IsPrivate = false,
                GlobalId = globalId
            });
        }

        void IEntitySpawnService.Despawn(VhId sourceId, EntityLocalId localId)
        {
            EntityGlobalId globalId = this._entityQueryService.GetGlobalId(localId);
            this._strategy.Publish(NameSpace<EntitySpawnService>.Instance.Create(sourceId), new DespawnEntity()
            {
                IsPrivate = false,
                GlobalId = globalId
            });
        }

        EntityLocalId IPrivateEntitySpawnService.Spawn(VhId sourceId, Key<IEntityTemplate> entityTemplateKey, EntityGlobalId globalId)
        {
            this._strategy.Publish(NameSpace<EntitySpawnService>.Instance.Create(sourceId), new SpawnEntity()
            {
                IsPrivate = true,
                TemplateKey = entityTemplateKey,
                GlobalId = globalId
            });

            return this._entityQueryService.GetLocalId(globalId);
        }

        EntityLocalId IPrivateEntitySpawnService.Spawn(VhId sourceId, Key<IEntityTemplate> entityTemplateKey, EntityGlobalId globalId, EntityInitializerDelegate initializer)
        {
            this._strategy.Publish(NameSpace<EntitySpawnService>.Instance.Create(sourceId), new SpawnEntity<EntityInitializerDelegate>()
            {
                IsPrivate = true,
                TemplateKey = entityTemplateKey,
                GlobalId = globalId,
                Initializer = initializer
            });

            return this._entityQueryService.GetLocalId(globalId);
        }

        void IPrivateEntitySpawnService.Despawn(VhId sourceId, EntityGlobalId globalId)
        {
            this._strategy.Publish(NameSpace<EntitySpawnService>.Instance.Create(sourceId), new DespawnEntity()
            {
                IsPrivate = true,
                GlobalId = globalId
            });
        }
    }
}