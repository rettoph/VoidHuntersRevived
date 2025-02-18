using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Events;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public partial class EntitySpawnService(
        IStepEventService eventService,
        IEntityQueryService entityQueryService
    ) : IEntitySpawnService,
        IPrivateEntitySpawnService
    {
        private readonly IStepEventService _eventService = eventService;
        private readonly IEntityQueryService _entityQueryService = entityQueryService;

        EntityLocalId IEntitySpawnService.Spawn(VhId sourceId, Key<IEntityTemplate> entityTemplateKey, EntityGlobalId globalId)
        {
            this._eventService.Publish(NameSpace<EntitySpawnService>.Instance.Create(sourceId), new SpawnEntity()
            {
                IsPrivate = false,
                TemplateKey = entityTemplateKey,
                GlobalId = globalId
            });

            return this._entityQueryService.GetLocalId(globalId);
        }

        EntityLocalId IEntitySpawnService.Spawn(VhId sourceId, Key<IEntityTemplate> entityTemplateKey, EntityGlobalId globalId, EntityInitializerDelegate initializer)
        {
            this._eventService.Publish(NameSpace<EntitySpawnService>.Instance.Create(sourceId), new SpawnEntity<EntityInitializerDelegate>()
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
            this._eventService.Publish(NameSpace<EntitySpawnService>.Instance.Create(sourceId), new DespawnEntity()
            {
                IsPrivate = false,
                GlobalId = globalId
            });
        }

        void IEntitySpawnService.Despawn(VhId sourceId, EntityLocalId localId)
        {
            EntityGlobalId globalId = this._entityQueryService.GetGlobalId(localId);
            this._eventService.Publish(NameSpace<EntitySpawnService>.Instance.Create(sourceId), new DespawnEntity()
            {
                IsPrivate = false,
                GlobalId = globalId
            });
        }

        EntityLocalId IPrivateEntitySpawnService.Spawn(VhId sourceId, Key<IEntityTemplate> entityTemplateKey, EntityGlobalId globalId)
        {
            this._eventService.Publish(NameSpace<EntitySpawnService>.Instance.Create(sourceId), new SpawnEntity()
            {
                IsPrivate = true,
                TemplateKey = entityTemplateKey,
                GlobalId = globalId
            });

            return this._entityQueryService.GetLocalId(globalId);
        }

        EntityLocalId IPrivateEntitySpawnService.Spawn(VhId sourceId, Key<IEntityTemplate> entityTemplateKey, EntityGlobalId globalId, EntityInitializerDelegate initializer)
        {
            this._eventService.Publish(NameSpace<EntitySpawnService>.Instance.Create(sourceId), new SpawnEntity<EntityInitializerDelegate>()
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
            this._eventService.Publish(NameSpace<EntitySpawnService>.Instance.Create(sourceId), new DespawnEntity()
            {
                IsPrivate = true,
                GlobalId = globalId
            });
        }
    }
}