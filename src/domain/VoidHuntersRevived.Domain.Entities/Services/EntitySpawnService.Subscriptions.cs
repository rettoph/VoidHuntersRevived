using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Events;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public partial class EntitySpawnService :
        IEventEngine<SpawnEntity>,
        IEventEngine<SpawnEntity<EntityInitializerDelegate>>,
        IEventEngine<SoftSpawnEntity>,
        IRevertEventEngine<SpawnEntity>,
        IRevertEventEngine<SpawnEntity<EntityInitializerDelegate>>,
        IEventEngine<DespawnEntity>,
        IRevertEventEngine<DespawnEntity>,
        IEventEngine<HardDespawnEntity>
    {
        public void Process(VhId eventId, SpawnEntity data)
        {
            if (this._entityQueryService.TryGetLocalId(data.GlobalId, out EntityLocalId existingLocalId) == true)
            {
                this._logger.Warning("{MethodName}, Entity already exists. GlobalId = {GlobalId}, LocalId = {LocalId}", nameof(SpawnEntity), data.GlobalId, existingLocalId);
                return;
            }

            // Enqueue SoftSpawn entity event
            // This is enqueued before HardSpawn is executed
            // Spawns any other entities. This ensture the first entitiy SoftSpawn
            // event is called first every time.
            this.Strategy.Enqueue(new EventDto()
            {
                SourceId = eventId,
                Data = new SoftSpawnEntity()
                {
                    IsPrivate = true,
                    GlobalId = data.GlobalId
                }
            });

            ref EntityLocalId localId = ref this._entityQueryService.AddLocalId(data.GlobalId);
            IEntityTemplate template = this._entityTemplateService.GetByKey(data.TemplateKey);

            EntityInitializer initializer = template.HardSpawnInstanceEntity(eventId, data.GlobalId, out localId);
            this._entityQueryService.AddGlobalId(localId, data.GlobalId);

            this._logger.Verbose("{MethodName}, Hard spawned entity. GlobalId = {GlobalId}, LocalId = {LocalId}, Template = {Template}", nameof(SpawnEntity), data.GlobalId, localId, template.Key.Name);
        }

        public void Process(VhId eventId, SpawnEntity<EntityInitializerDelegate> data)
        {
            if (this._entityQueryService.TryGetLocalId(data.GlobalId, out EntityLocalId existingLocalId) == true)
            {
                this._logger.Warning("{MethodName}, Entity already exists. GlobalId = {GlobalId}, LocalId = {LocalId}", nameof(SpawnEntity), data.GlobalId, existingLocalId);
                return;
            }

            // Enqueue SoftSpawn entity event
            // This is enqueued before HardSpawn is executed
            // Spawns any other entities. This ensture the first entitiy SoftSpawn
            // event is called first every time.
            this.Strategy.Enqueue(new EventDto()
            {
                SourceId = eventId,
                Data = new SoftSpawnEntity()
                {
                    IsPrivate = true,
                    GlobalId = data.GlobalId
                }
            });

            ref EntityLocalId localId = ref this._entityQueryService.AddLocalId(data.GlobalId);
            IEntityTemplate template = this._entityTemplateService.GetByKey(data.TemplateKey);

            EntityInitializer initializer = template.HardSpawnInstanceEntity(eventId, data.GlobalId, out localId);
            this._entityQueryService.AddGlobalId(localId, data.GlobalId);

            InitializingEntity entity = new(in localId, data.GlobalId, ref initializer, in template);
            data.Initializer.Invoke(this._entityService, in entity);

            this._logger.Verbose("{MethodName}, Hard spawned entity with initializer. GlobalId = {GlobalId}, LocalId = {LocalId}, Template = {Template}", nameof(SpawnEntity), data.GlobalId, localId, template.Key.Name);
        }

        public void Process(VhId eventId, SoftSpawnEntity data)
        {
            if (this._entityQueryService.TryGetLocalId(data.GlobalId, out EntityLocalId localId) == false)
            {
                this._logger.Warning("{MethodName}, Unknown GlobalId. GlobalId = {GlobalId}", nameof(SoftSpawnEntity), data.GlobalId);
                return;
            }

            ref EntityStatus status = ref this._entityQueryService.QueryByEGID<EntityStatus>(localId.Value, out GroupIndex groupIndex, out bool exists);
            if (exists == false)
            {
                this._logger.Warning("{MethodName}, Entity not found. GlobalId = {GlobalId}, LocalId = {LocalId}", nameof(SoftSpawnEntity), data.GlobalId, localId);
                return;
            }

            if (status.IncrementSoftSpawnCount() == false || status.Value != EntityStatusEnum.HardSpawned)
            {
                this._logger.Warning("{MethodName}, Invalid entity state. GlobalId = {GlobalId}, LocalId = {LocalId}, Status = {Status}, Count = {Count}", nameof(SoftSpawnEntity), data.GlobalId, localId, status.Value, status.Count);
                return;
            }

            Key<IEntityTemplate> templateKey = this._entityQueryService.QueryByGroupIndex<Common.Components.EntityTemplate>(groupIndex).Key;
            Entity entity = new(groupIndex.Index, localId, data.GlobalId);

            this._logger.Verbose("{MethodName}, GlobalId = {GlobalId}, LocalId = {LocalId}, Template = {Template}", nameof(SoftSpawnEntity), data.GlobalId, localId, templateKey.Name);
            this._entityTemplateService.GetByKey(templateKey).SoftSpawnInstanceEntity(in eventId, in entity, ref status);
            status.Value = EntityStatusEnum.SoftSpawned;
        }

        public void Revert(VhId eventId, SpawnEntity data)
        {
            this.InternalRevert(eventId, data);
        }

        public void Revert(VhId eventId, SpawnEntity<EntityInitializerDelegate> data)
        {
            this.InternalRevert(eventId, data);
        }

        public void InternalRevert(VhId eventId, SpawnEntity data)
        {
            if (this._entityQueryService.TryGetLocalId(data.GlobalId, out EntityLocalId localId) == false)
            {
                this._logger.Warning("(Revert) {MethodName}, Unknown GlobalId. GlobalId = {GlobalId}", nameof(SpawnEntity), data.GlobalId);
                return;
            }

            ref EntityStatus status = ref this._entityQueryService.QueryByEGID<EntityStatus>(localId.Value, out GroupIndex groupIndex, out bool exists);
            if (exists == false)
            {
                this._logger.Warning("{MethodName}, Entity not found. GlobalId = {GlobalId}, LocalId = {LocalId}", nameof(SpawnEntity), data.GlobalId, localId);
                return;
            }

            Key<IEntityTemplate> templateKey = this._entityQueryService.QueryByGroupIndex<Common.Components.EntityTemplate>(groupIndex).Key;
            IEntityTemplate template = this._entityTemplateService.GetByKey(templateKey);
            Entity entity = new(groupIndex.Index, localId, data.GlobalId);

            this.SoftDespawn(eventId, ref status, ref entity, template);

            this.Strategy.Enqueue(new EventDto()
            {
                SourceId = eventId,
                Data = new HardDespawnEntity()
                {
                    IsPrivate = true,
                    IsPredictable = true,
                    GlobalId = data.GlobalId
                }
            });
        }

        public void Process(VhId eventId, DespawnEntity data)
        {
            if (this._entityQueryService.TryGetLocalId(data.GlobalId, out EntityLocalId localId) == false)
            {
                this._logger.Warning("(Revert) {MethodName}, Unknown GlobalId. GlobalId = {GlobalId}", nameof(SpawnEntity), data.GlobalId);
                return;
            }

            ref EntityStatus status = ref this._entityQueryService.QueryByEGID<EntityStatus>(localId.Value, out GroupIndex groupIndex, out bool exists);
            if (exists == false)
            {
                this._logger.Warning("{MethodName}, Entity not found. GlobalId = {GlobalId}, LocalId = {LocalId}", nameof(SpawnEntity), data.GlobalId, localId);
                return;
            }

            Key<IEntityTemplate> templateKey = this._entityQueryService.QueryByGroupIndex<Common.Components.EntityTemplate>(groupIndex).Key;
            IEntityTemplate template = this._entityTemplateService.GetByKey(templateKey);
            Entity entity = new(groupIndex.Index, localId, data.GlobalId);

            this.SoftDespawn(eventId, ref status, ref entity, template);

            this.Strategy.Enqueue(new EventDto()
            {
                SourceId = eventId,
                Data = new HardDespawnEntity()
                {
                    IsPrivate = data.IsPrivate,
                    IsPredictable = data.IsPrivate,
                    GlobalId = data.GlobalId
                }
            });
        }

        public void Process(VhId eventId, HardDespawnEntity data)
        {
            if (this._entityQueryService.TryGetLocalId(data.GlobalId, out EntityLocalId localId) == false)
            {
                this._logger.Warning("(Revert) {MethodName}, Unknown GlobalId. GlobalId = {GlobalId}", nameof(SpawnEntity), data.GlobalId);
                return;
            }

            ref EntityStatus status = ref this._entityQueryService.QueryByEGID<EntityStatus>(localId.Value, out GroupIndex groupIndex, out bool exists);
            if (exists == false)
            {
                this._logger.Warning("{MethodName}, Entity not found. GlobalId = {GlobalId}, LocalId = {LocalId}", nameof(SpawnEntity), data.GlobalId, localId);
                return;
            }

            Key<IEntityTemplate> templateKey = this._entityQueryService.QueryByGroupIndex<Common.Components.EntityTemplate>(groupIndex).Key;
            IEntityTemplate template = this._entityTemplateService.GetByKey(templateKey);
            Entity entity = new(groupIndex.Index, localId, data.GlobalId);

            this.SoftDespawn(eventId, ref status, ref entity, template);

            this._logger.Verbose("{MethodName}, GlobalId = {GlobalId}, LocalId = {LocalId}, Template = {Template}", nameof(HardDespawnEntity), data.GlobalId, localId, templateKey.Name);
            template.HardDespawnInstanceEntity(in eventId, in entity, ref status);
            this._entityQueryService.Remove(data.GlobalId);
            status.Value = EntityStatusEnum.HardDespawned;
        }

        public void Revert(VhId eventId, DespawnEntity data)
        {
            if (this._entityQueryService.TryGetLocalId(data.GlobalId, out EntityLocalId localId) == false)
            {
                this._logger.Warning("(Revert) {MethodName}, Unknown GlobalId. GlobalId = {GlobalId}", nameof(SpawnEntity), data.GlobalId);
                return;
            }

            ref EntityStatus status = ref this._entityQueryService.QueryByEGID<EntityStatus>(localId.Value, out GroupIndex groupIndex, out bool exists);
            if (exists == false)
            {
                this._logger.Warning("{MethodName}, Entity not found. GlobalId = {GlobalId}, LocalId = {LocalId}", nameof(SpawnEntity), data.GlobalId, localId);
                return;
            }

            if (status.IncrementSoftSpawnCount() == false || status.Value != EntityStatusEnum.SoftDespawned)
            {
                this._logger.Warning("(Revert) {MethodName}, Invalid entity state. GlobalId = {GlobalId}, LocalId = {LocalId}, Status = {Status}, Count = {Count}", nameof(DespawnEntity), data.GlobalId, localId, status.Value, status.Count);
                return;
            }

            Key<IEntityTemplate> templateKey = this._entityQueryService.QueryByGroupIndex<Common.Components.EntityTemplate>(groupIndex).Key;
            IEntityTemplate template = this._entityTemplateService.GetByKey(templateKey);
            Entity entity = new(groupIndex.Index, localId, data.GlobalId);

            this._logger.Verbose("(Revert) {MethodName}, Soft spawn. GlobalId = {GlobalId}, LocalId = {LocalId}", nameof(DespawnEntity), data.GlobalId, localId, status.Value);
            template.SoftSpawnInstanceEntity(in eventId, in entity, ref status);
            status.Value = EntityStatusEnum.SoftSpawned;
        }

        private void SoftDespawn(VhId sourceEventId, ref EntityStatus status, ref Entity entity, IEntityTemplate template)
        {
            if (status.IncrementSoftDespawnCount() == false)
            {
                this._logger.Warning("{MethodName}, SoftDespawn overflow. GlobalId = {GlobalId}, LocalId = {LocalId}", nameof(SoftDespawn), entity.GlobalId, entity.LocalId);
                return;
            }

            if (status.Value != EntityStatusEnum.SoftSpawned)
            {
                this._logger.Warning("{MethodName}, Invalid state. GlobalId = {GlobalId}, LocalId = {LocalId}, Status = {Status}", nameof(SoftDespawn), entity.GlobalId, entity.LocalId, status.Value);
                return;
            }

            this._logger.Verbose("{MethodName}, GlobalId = {GlobalId}, LocalId = {LocalId}, Template = {Template}", nameof(SoftDespawn), entity.GlobalId, entity.LocalId, template.Key.Name);
            template.SoftDespawnInstanceEntity(in sourceEventId, in entity, ref status);
            status.Value = EntityStatusEnum.SoftDespawned;
        }
    }
}