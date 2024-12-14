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
        IEventEngine<HardSpawnEntity>,
        IEventEngine<HardSpawnEntity<EntityInitializerDelegate>>,
        IEventEngine<SoftSpawnEntity>,
        IRevertEventEngine<SpawnEntity>,
        IRevertEventEngine<SpawnEntity<EntityInitializerDelegate>>,
        IEventEngine<DespawnEntity>,
        IRevertEventEngine<DespawnEntity>,
        IEventEngine<SoftDespawnEntity>,
        IEventEngine<HardDespawnEntity>
    {
        public void Process(VhId eventId, SpawnEntity data)
        {
            if (_entityQueryService.TryGetId(data.GlobalId.Value, out EntityId id) == true)
            {
                ref EntityStatus status = ref _entityQueryService.QueryByEGID<EntityStatus>(id.EGID);
                status.Increment(EntityModificationTypeEnum.Spawned);

                return;
            }

            // Enqueue SoftSpawn entity event
            // This is enqueued before HardSpawn is published in case the initializer
            // Spawns any other entities. This ensture the first entitiy SoftSpawn
            // event is called first every time.
            this.Strategy.Enqueue(new EventDto()
            {
                SourceId = eventId,
                Data = new SoftSpawnEntity()
                {
                    IsPrivate = data.IsPrivate,
                    GlobalId = data.GlobalId
                }
            });

            this.Strategy.Publish(new EventDto()
            {
                SourceId = eventId,
                Data = new HardSpawnEntity()
                {
                    IsPrivate = data.IsPrivate,
                    GlobalId = data.GlobalId,
                    TemplateKey = data.TemplateKey
                }
            });
        }

        public void Process(VhId eventId, SpawnEntity<EntityInitializerDelegate> data)
        {
            if (_entityQueryService.TryGetId(data.GlobalId.Value, out EntityId id) == true)
            {
                ref EntityStatus status = ref _entityQueryService.QueryByEGID<EntityStatus>(id.EGID);
                status.Increment(EntityModificationTypeEnum.Spawned);

                return;
            }

            // Enqueue SoftSpawn entity event
            // This is enqueued before HardSpawn is published in case the initializer
            // Spawns any other entities. This ensture a first in first out order of
            // SoftSpawn events
            this.Strategy.Enqueue(new EventDto()
            {
                SourceId = eventId,
                Data = new SoftSpawnEntity()
                {
                    IsPrivate = data.IsPrivate,
                    GlobalId = data.GlobalId
                }
            });

            // Publish HardSpawn event immidiately
            this.Strategy.Publish(new EventDto()
            {
                SourceId = eventId,
                Data = new HardSpawnEntity<EntityInitializerDelegate>()
                {
                    IsPrivate = data.IsPrivate,
                    GlobalId = data.GlobalId,
                    TemplateKey = data.TemplateKey,
                    Initializer = data.Initializer
                }
            });
        }

        public void Process(VhId eventId, HardSpawnEntity data)
        {
            ref EntityLocalId localId = ref _entityQueryService.AddLocalId(data.GlobalId);
            EntityInitializer initializer = _entityTemplateService.GetByKey(data.TemplateKey).HardSpawnInstanceEntity(eventId, data.GlobalId, out localId);
            _entityQueryService.AddGlobalId(localId, data.GlobalId);
        }

        public void Process(VhId eventId, HardSpawnEntity<EntityInitializerDelegate> data)
        {
            ref EntityLocalId localId = ref _entityQueryService.AddLocalId(data.GlobalId);
            IEntityTemplate template = _entityTemplateService.GetByKey(data.TemplateKey);

            _logger.Verbose("HardSpawnInstanceEntity - GlobalId = {GlobalId}, Template = {Template}", data.GlobalId, template.Key.Name);
            EntityInitializer initializer = template.HardSpawnInstanceEntity(eventId, data.GlobalId, out localId);
            _entityQueryService.AddGlobalId(localId, data.GlobalId);

            InitializingEntity entity = new(in localId, data.GlobalId, ref initializer, in template);
            data.Initializer.Invoke(_entityService, in entity);
        }

        public void Process(VhId eventId, SoftSpawnEntity data)
        {
            if (_entityQueryService.TryGetLocalId(data.GlobalId, out EntityLocalId localId) == false)
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Unable to soft spawn entity, unknown GlobalId {GlobalId}", nameof(EntitySpawnService), nameof(Process), nameof(SoftSpawnEntity), data.GlobalId);
                return;
            }

            ref EntityStatus status = ref _entityQueryService.QueryByEGID<EntityStatus>(localId.Value, out GroupIndex groupIndex, out bool exists);
            if (exists == false || status.Value != EntityStatusEnum.HardSpawned)
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - LocalId = {LocalId}, Exists = {Exists}, Status = {Status}", nameof(EntitySpawnService), nameof(Process), nameof(SoftSpawnEntity), localId, exists, exists ? status.Value : null);
                return;
            }

            Key<IEntityTemplate> templateKey = _entityQueryService.QueryByGroupIndex<Common.Components.EntityTemplate>(groupIndex).Key;
            Entity entity = new(groupIndex.Index, localId, data.GlobalId);

            _logger.Verbose("SoftSpawnInstanceEntity - GlobalId = {GlobalId}, Template = {Template}", data.GlobalId, templateKey.Name);
            _entityTemplateService.GetByKey(templateKey).SoftSpawnInstanceEntity(in eventId, in entity, ref status);
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
            if (_entityQueryService.TryGetLocalId(data.GlobalId, out EntityLocalId localId) == false)
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Unable to soft spawn entity, unknown GlobalId {GlobalId}", nameof(EntitySpawnService), nameof(InternalRevert), nameof(SpawnEntity), data.GlobalId);
                return;
            }

            ref EntityStatus status = ref _entityQueryService.QueryByEGID<EntityStatus>(localId.Value, out _, out bool exists);
            if (exists == false)
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - LocalId = {LocalId}, Exists = {Exists}, Status = {Status}", nameof(EntitySpawnService), nameof(InternalRevert), nameof(SpawnEntity), localId, exists, exists ? status.Value : null);
                return;
            }

            int spawnCount = 0;
            if ((spawnCount = status.Increment(EntityModificationTypeEnum.Despawned)) == 0)
            {
                this.Strategy.Enqueue(new EventDto()
                {
                    SourceId = eventId,
                    Data = new SoftDespawnEntity()
                    {
                        IsPrivate = true,
                        GlobalId = data.GlobalId
                    }
                });
            }
            else
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - LocalId = {LocalId}, Exists = {Exists}, Status = {Status}, SpawnCount = {SpawnCount}", nameof(EntitySpawnService), nameof(InternalRevert), nameof(SpawnEntity), localId, exists, exists ? status.Value : null, spawnCount);
            }

            // TODO: Investigate why the HardDespawn event is published despite the SoftDespawn being locked behind the Despawn counter
            // I dont remember if this was by design or if its just a bug
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

            status.Value = EntityStatusEnum.RevertSpawnEnqueued;
        }

        public void Process(VhId eventId, DespawnEntity data)
        {
            if (_entityQueryService.TryGetLocalId(data.GlobalId, out EntityLocalId localId) == false)
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Unable to despawn entity, unknown GlobalId {GlobalId}", nameof(EntitySpawnService), nameof(Process), nameof(DespawnEntity), data.GlobalId);
                return;
            }

            ref EntityStatus status = ref _entityQueryService.QueryByEGID<EntityStatus>(localId.Value, out GroupIndex groupIndex, out bool exists);
            int spawnedCount = 0;
            if (exists == false || (spawnedCount = status.Increment(EntityModificationTypeEnum.Despawned)) != 0)
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - LocalId = {LocalId}, Exists = {Exists}, Status = {Status}, SpawnedCount = {SpawnedCount}", nameof(EntitySpawnService), nameof(Process), nameof(DespawnEntity), localId, exists, exists ? status.Value : null, spawnedCount);
                return;
            }


            this.Strategy.Enqueue(new EventDto()
            {
                SourceId = eventId,
                Data = new SoftDespawnEntity()
                {
                    IsPrivate = data.IsPrivate,
                    GlobalId = data.GlobalId
                }
            });

            this.Strategy.Enqueue(new EventDto()
            {
                SourceId = eventId,
                Data = new HardDespawnEntity()
                {
                    IsPrivate = data.IsPrivate,
                    IsPredictable = false,
                    GlobalId = data.GlobalId
                }
            });

            status.Value = EntityStatusEnum.SoftDespawnEnqueued;
        }

        public void Process(VhId eventId, SoftDespawnEntity data)
        {
            if (_entityQueryService.TryGetLocalId(data.GlobalId, out EntityLocalId localId) == false)
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Unable to soft despawn entity. GlobalId = {GlobalId}", nameof(EntitySpawnService), nameof(Process), nameof(SoftDespawnEntity), data.GlobalId);
                return;
            }

            ref EntityStatus status = ref _entityQueryService.QueryByEGID<EntityStatus>(localId.Value, out GroupIndex groupIndex, out bool exists);
            if (exists == false || status.Value != EntityStatusEnum.SoftDespawnEnqueued)
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Unable to soft despawn entity. LocalId = {LocalId}, Exists = {Exists}, Status = {Status}", nameof(EntitySpawnService), nameof(Process), nameof(SoftDespawnEntity), localId, exists, exists ? status.Value : null);
                return;
            }

            Key<IEntityTemplate> templateKey = _entityQueryService.QueryByGroupIndex<Common.Components.EntityTemplate>(groupIndex).Key;
            Entity entity = new(groupIndex.Index, localId, data.GlobalId);

            _logger.Verbose("SoftDespawnInstanceEntity - GlobalId = {GlobalId}, Template = {Template}", data.GlobalId, templateKey.Name);
            _entityTemplateService.GetByKey(templateKey).SoftDespawnInstanceEntity(in eventId, in entity, ref status);
            status.Value = EntityStatusEnum.SoftDespawned;
        }

        public void Process(VhId eventId, HardDespawnEntity data)
        {
            if (_entityQueryService.TryGetLocalId(data.GlobalId, out EntityLocalId localId) == false)
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Unable to hard despawn entity. GlobalId = {GlobalId}", nameof(EntitySpawnService), nameof(Process), nameof(HardDespawnEntity), data.GlobalId);
                return;
            }

            ref EntityStatus status = ref _entityQueryService.QueryByEGID<EntityStatus>(localId.Value, out GroupIndex groupIndex, out bool exists);

            if (exists == false)
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Unable to hard despawn entity. LocalId = {LocalId}, Exists = {Exists}, Status = {Status}", nameof(EntitySpawnService), nameof(Process), nameof(HardDespawnEntity), localId, exists, exists ? status.Value : null);
                return;
            }

            Key<IEntityTemplate> templateKey = _entityQueryService.QueryByGroupIndex<Common.Components.EntityTemplate>(groupIndex).Key;
            IEntityTemplate descriptorEngine = _entityTemplateService.GetByKey(templateKey);
            Entity entity = new(groupIndex.Index, localId, data.GlobalId);

            if (status.Value < EntityStatusEnum.SoftDespawned)
            { // Ensure an entity gets soft despawned if it hasn't been already
                descriptorEngine.SoftDespawnInstanceEntity(in eventId, in entity, ref status);
                status.Value = EntityStatusEnum.SoftDespawned;
            }

            _logger.Verbose("HardDespawnInstanceEntity - GlobalId = {GlobalId}, Template = {Template}", data.GlobalId, templateKey.Name);
            descriptorEngine.HardDespawnInstanceEntity(in eventId, in entity, ref status);
            _entityQueryService.Remove(data.GlobalId);
            status.Value = EntityStatusEnum.HardDespawned;
        }

        public void Revert(VhId eventId, DespawnEntity data)
        {
            if (_entityQueryService.TryGetLocalId(data.GlobalId, out EntityLocalId localId) == false)
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Unable to revert despawn entity, unknown GlobalId {GlobalId}", nameof(EntitySpawnService), nameof(Revert), nameof(DespawnEntity), data.GlobalId);
                return;
            }

            ref EntityStatus status = ref _entityQueryService.QueryByEGID<EntityStatus>(localId.Value, out GroupIndex groupIndex, out bool exists);

            int spawnedCount = 0;
            if (exists == false || (spawnedCount = status.Increment(EntityModificationTypeEnum.Spawned)) != 1)
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - LocalId = {LocalId}, Exists = {Exists}, Status = {Status}, SpawnedCount = {SpawnedCount}", nameof(EntitySpawnService), nameof(Revert), nameof(DespawnEntity), localId, exists, exists ? status.Value : null, spawnedCount);
                return;
            }

            this.Strategy.Enqueue(new EventDto()
            {
                SourceId = eventId,
                Data = new SoftSpawnEntity()
                {
                    IsPrivate = true,
                    GlobalId = data.GlobalId
                }
            });

            status.Value = EntityStatusEnum.HardSpawned;
        }
    }
}
