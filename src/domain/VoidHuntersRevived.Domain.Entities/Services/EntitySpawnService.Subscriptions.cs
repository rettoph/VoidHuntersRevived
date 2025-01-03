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
        IEventEngine<SoftDespawnEntity>,
        IEventEngine<HardDespawnEntity>
    {
        public void Process(VhId eventId, SpawnEntity data)
        {
            if (_entityQueryService.TryGetLocalId(data.GlobalId, out EntityLocalId existingLocalId) == true)
            {
                _logger.Warning("{MethodName}, Entity already exists. GlobalId = {GlobalId}, LocalId = {LocalId}", nameof(SpawnEntity), data.GlobalId, existingLocalId);
                ref EntityStatus status = ref _entityQueryService.QueryByEGID<EntityStatus>(existingLocalId.Value);
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
                    IsPrivate = true,
                    GlobalId = data.GlobalId
                }
            });

            ref EntityLocalId localId = ref _entityQueryService.AddLocalId(data.GlobalId);
            IEntityTemplate template = _entityTemplateService.GetByKey(data.TemplateKey);

            EntityInitializer initializer = template.HardSpawnInstanceEntity(eventId, data.GlobalId, out localId);
            _entityQueryService.AddGlobalId(localId, data.GlobalId);

            _logger.Verbose("{MethodName}, Hard spawned entity. GlobalId = {GlobalId}, LocalId = {LocalId}, Template = {Template}", nameof(SpawnEntity), data.GlobalId, localId, template.Key.Name);
        }

        public void Process(VhId eventId, SpawnEntity<EntityInitializerDelegate> data)
        {
            if (_entityQueryService.TryGetLocalId(data.GlobalId, out EntityLocalId existingLocalId) == true)
            {
                _logger.Warning("{MethodName}, Entity already exists. GlobalId = {GlobalId}, LocalId = {LocalId}", nameof(SpawnEntity), data.GlobalId, existingLocalId);

                ref EntityStatus status = ref _entityQueryService.QueryByEGID<EntityStatus>(existingLocalId.Value);
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
                    IsPrivate = true,
                    GlobalId = data.GlobalId
                }
            });

            ref EntityLocalId localId = ref _entityQueryService.AddLocalId(data.GlobalId);
            IEntityTemplate template = _entityTemplateService.GetByKey(data.TemplateKey);

            EntityInitializer initializer = template.HardSpawnInstanceEntity(eventId, data.GlobalId, out localId);
            _entityQueryService.AddGlobalId(localId, data.GlobalId);

            InitializingEntity entity = new(in localId, data.GlobalId, ref initializer, in template);
            data.Initializer.Invoke(_entityService, in entity);

            _logger.Verbose("{MethodName}, Hard spawned entity with initializer. GlobalId = {GlobalId}, LocalId = {LocalId}, Template = {Template}", nameof(SpawnEntity), data.GlobalId, localId, template.Key.Name);
        }

        public void Process(VhId eventId, SoftSpawnEntity data)
        {
            if (_entityQueryService.TryGetLocalId(data.GlobalId, out EntityLocalId localId) == false)
            {
                _logger.Warning("{MethodName}, Unknown GlobalId. GlobalId = {GlobalId}", nameof(SoftSpawnEntity), data.GlobalId);
                return;
            }

            ref EntityStatus status = ref _entityQueryService.QueryByEGID<EntityStatus>(localId.Value, out GroupIndex groupIndex, out bool exists);
            if (exists == false || status.Value != EntityStatusEnum.HardSpawned)
            {
                _logger.Warning("{MethodName}, Invalid entity state. GlobalId = {GlobalId}, LocalId = {LocalId}, Exists = {Exists}, Status = {Status}", nameof(SoftSpawnEntity), data.GlobalId, localId, exists, exists ? status.Value : null);
                return;
            }

            Key<IEntityTemplate> templateKey = _entityQueryService.QueryByGroupIndex<Common.Components.EntityTemplate>(groupIndex).Key;
            Entity entity = new(groupIndex.Index, localId, data.GlobalId);

            _logger.Verbose("{MethodName}, GlobalId = {GlobalId}, LocalId = {LocalId}, Template = {Template}", nameof(SoftSpawnEntity), data.GlobalId, localId, templateKey.Name);
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
                _logger.Warning("(Revert){MethodName}, Unknown GlobalId. GlobalId = {GlobalId}", nameof(SpawnEntity), data.GlobalId);
                return;
            }

            ref EntityStatus status = ref _entityQueryService.QueryByEGID<EntityStatus>(localId.Value, out _, out bool exists);
            if (exists == false)
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - LocalId = {LocalId}, Exists = {Exists}, Status = {Status}", nameof(EntitySpawnService), nameof(InternalRevert), nameof(SpawnEntity), localId, exists, exists ? status.Value : null);
                return;
            }

            int spawnCount = 0;
            if ((spawnCount = status.Increment(EntityModificationTypeEnum.Despawned)) != 0)
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - LocalId = {LocalId}, Exists = {Exists}, Status = {Status}, SpawnCount = {SpawnCount}", nameof(EntitySpawnService), nameof(InternalRevert), nameof(SpawnEntity), localId, exists, exists ? status.Value : null, spawnCount);
            }

            this.Strategy.Enqueue(new EventDto()
            {
                SourceId = eventId,
                Data = new SoftDespawnEntity()
                {
                    IsPrivate = true,
                    GlobalId = data.GlobalId
                }
            });

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
                _logger.Warning("{MethodName}, Unknown GlobalId. GlobalId = {GlobalId}", nameof(DespawnEntity), data.GlobalId);
                return;
            }

            ref EntityStatus status = ref _entityQueryService.QueryByEGID<EntityStatus>(localId.Value, out GroupIndex groupIndex, out bool exists);
            int spawnedCount = 0;
            if (exists == false || (spawnedCount = status.Increment(EntityModificationTypeEnum.Despawned)) != 0)
            {
                _logger.Warning("{MethodName}, Invalid entity state. GlobalId = {GlobalId}, LocalId = {LocalId}, Exists = {Exists}, Status = {Status}, SpawnCount = {SpawnCount}", nameof(DespawnEntity), data.GlobalId, localId, exists, exists ? status.Value : null, spawnedCount);
                return;
            }


            this.Strategy.Enqueue(new EventDto()
            {
                SourceId = eventId,
                Data = new SoftDespawnEntity()
                {
                    IsPrivate = true,
                    GlobalId = data.GlobalId
                }
            });

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

            status.Value = EntityStatusEnum.SoftDespawnEnqueued;
        }

        public void Process(VhId eventId, SoftDespawnEntity data)
        {
            if (_entityQueryService.TryGetLocalId(data.GlobalId, out EntityLocalId localId) == false)
            {
                _logger.Warning("{MethodName}, Unknown GlobalId. GlobalId = {GlobalId}", nameof(SoftDespawnEntity), data.GlobalId);
                return;
            }

            ref EntityStatus status = ref _entityQueryService.QueryByEGID<EntityStatus>(localId.Value, out GroupIndex groupIndex, out bool exists);
            if (exists == false || status.Value != EntityStatusEnum.SoftDespawnEnqueued)
            {
                _logger.Warning("{MethodName}, Invalid entity state. GlobalId = {GlobalId}, LocalId = {LocalId}, Exists = {Exists}, Status = {Status}", nameof(SoftDespawnEntity), data.GlobalId, localId, exists, exists ? status.Value : null);
                return;
            }

            Key<IEntityTemplate> templateKey = _entityQueryService.QueryByGroupIndex<Common.Components.EntityTemplate>(groupIndex).Key;
            Entity entity = new(groupIndex.Index, localId, data.GlobalId);

            _logger.Verbose("{MethodName}, GlobalId = {GlobalId}, LocalId = {LocalId}, Template = {Template}", data.GlobalId, localId, templateKey.Name);
            _entityTemplateService.GetByKey(templateKey).SoftDespawnInstanceEntity(in eventId, in entity, ref status);
            status.Value = EntityStatusEnum.SoftDespawned;
        }

        public void Process(VhId eventId, HardDespawnEntity data)
        {
            if (_entityQueryService.TryGetLocalId(data.GlobalId, out EntityLocalId localId) == false)
            {
                _logger.Warning("{MethodName}, Unknown GlobalId. GlobalId = {GlobalId}", nameof(HardDespawnEntity), data.GlobalId);
                return;
            }

            ref EntityStatus status = ref _entityQueryService.QueryByEGID<EntityStatus>(localId.Value, out GroupIndex groupIndex, out bool exists);

            if (exists == false)
            {
                _logger.Warning("{MethodName}, Invalid entity state. GlobalId = {GlobalId}, LocalId = {LocalId}, Exists = {Exists}, Status = {Status}", nameof(HardDespawnEntity), data.GlobalId, localId, exists, exists ? status.Value : null);
                return;
            }

            Key<IEntityTemplate> templateKey = _entityQueryService.QueryByGroupIndex<Common.Components.EntityTemplate>(groupIndex).Key;
            IEntityTemplate template = _entityTemplateService.GetByKey(templateKey);
            Entity entity = new(groupIndex.Index, localId, data.GlobalId);

            if (status.Value < EntityStatusEnum.SoftDespawned)
            { // Ensure an entity gets soft despawned if it hasn't been already
                template.SoftDespawnInstanceEntity(in eventId, in entity, ref status);
                status.Value = EntityStatusEnum.SoftDespawned;
            }

            _logger.Verbose("{MethodName}, GlobalId = {GlobalId}, LocalId = {LocalId}, Template = {Template}", nameof(HardDespawnEntity), data.GlobalId, localId, templateKey.Name);
            template.HardDespawnInstanceEntity(in eventId, in entity, ref status);
            _entityQueryService.Remove(data.GlobalId);
            status.Value = EntityStatusEnum.HardDespawned;
        }

        public void Revert(VhId eventId, DespawnEntity data)
        {
            if (_entityQueryService.TryGetLocalId(data.GlobalId, out EntityLocalId localId) == false)
            {
                _logger.Warning("(Revert) {MethodName}, Unknown GlobalId. GlobalId = {GlobalId}", nameof(DespawnEntity), data.GlobalId);
                return;
            }

            ref EntityStatus status = ref _entityQueryService.QueryByEGID<EntityStatus>(localId.Value, out GroupIndex groupIndex, out bool exists);

            int spawnedCount = 0;
            if (exists == false || (spawnedCount = status.Increment(EntityModificationTypeEnum.Spawned)) != 1)
            {
                _logger.Warning("(Revert) {MethodName}, Invalid entity state. GlobalId = {GlobalId}, LocalId = {LocalId}, Exists = {Exists}, Status = {Status}, SpawnCount = {SpawnCount}", nameof(DespawnEntity), data.GlobalId, localId, exists, exists ? status.Value : null, spawnedCount);
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
