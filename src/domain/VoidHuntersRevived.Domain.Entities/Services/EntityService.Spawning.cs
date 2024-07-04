using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Events;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Events;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public partial class EntityService :
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
        public EntityId Spawn(VhId sourceId, IEntityType type, VhId vhid)
        {
            this.Simulation.Publish(NameSpace<EntityService>.Instance.Create(sourceId), new SpawnEntity()
            {
                Type = type,
                VhId = vhid
            });

            return this.GetId(vhid);
        }

        public EntityId Spawn(VhId sourceId, IEntityType type, VhId vhid, EntityInitializerDelegate initializer)
        {
            this.Simulation.Publish(NameSpace<EntityService>.Instance.Create(sourceId), new SpawnEntity<EntityInitializerDelegate>()
            {
                Type = type,
                VhId = vhid,
                Initializer = initializer
            });

            return this.GetId(vhid);
        }

        public void Despawn(VhId sourceId, VhId vhid)
        {
            _logger.Verbose("{ClassName}::{MethodName} - EntityVhId = {EntityVhId}", nameof(EntityService), nameof(Despawn), vhid);

            this.Simulation.Publish(NameSpace<EntityService>.Instance.Create(sourceId), new DespawnEntity()
            {
                VhId = vhid
            });
        }

        public void Despawn(VhId sourceId, EntityId id)
        {
            this.Despawn(sourceId, id.VhId);
        }

        public void Flush()
        {
            _scheduler.SubmitEntities();
        }

        public bool IsSpawned(EntityId id)
        {
            if (this.TryQueryById<EntityStatus>(id, out EntityStatus status))
            {
                return status.IsSpawned;
            }

            return false;
        }

        public bool IsSpawned(EntityId id, out GroupIndex groupIndex)
        {
            if (this.TryQueryById<EntityStatus>(id, out groupIndex, out EntityStatus status))
            {
                return status.IsSpawned;
            }

            return false;
        }

        public bool IsSpawned(in GroupIndex groupIndex)
        {
            if (this.TryQueryByGroupIndex<EntityStatus>(in groupIndex, out EntityStatus status))
            {
                return status.IsSpawned;
            }

            return false;
        }

        public bool IsDespawned(EntityId id)
        {
            if (this.TryQueryById<EntityStatus>(id, out EntityStatus status))
            {
                return status.IsDespawned;
            }

            return false;
        }

        public bool IsDespawned(EntityId id, out GroupIndex groupIndex)
        {
            if (this.TryQueryById<EntityStatus>(id, out groupIndex, out EntityStatus status))
            {
                return status.IsDespawned;
            }

            return false;
        }

        public bool IsDespawned(in GroupIndex groupIndex)
        {
            if (this.TryQueryByGroupIndex<EntityStatus>(in groupIndex, out EntityStatus status))
            {
                return status.IsDespawned;
            }

            return false;
        }

        public void Process(VhId eventId, SpawnEntity data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{GenericType}> - EntityVhId = {EntityVhId}", nameof(EntityService), nameof(Process), nameof(SpawnEntity), data.VhId);

            if (this.TryGetId(data.VhId, out EntityId id) == true)
            {
                ref EntityStatus status = ref this.QueryById<EntityStatus>(id);
                status.Increment(EntityModificationTypeEnum.Spawned);

                return;
            }

            // Enqueue SoftSpawn entity event
            // This is enqueued before HardSpawn is published in case the initializer
            // Spawns any other entities. This ensture the first entitiy SoftSpawn
            // event is called first every time.
            this.Simulation.Enqueue(new EventDto()
            {
                SourceId = NameSpace<EntityService>.Instance.Create(eventId),
                Data = new SoftSpawnEntity()
                {
                    VhId = data.VhId
                }
            });

            this.Simulation.Publish(new EventDto()
            {
                SourceId = NameSpace<EntityService>.Instance.Create(eventId),
                Data = new HardSpawnEntity()
                {
                    VhId = data.VhId,
                    Type = data.Type
                }
            });
        }

        public void Process(VhId eventId, SpawnEntity<EntityInitializerDelegate> data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{GenericType}> - EntityVhId = {EntityVhId}", nameof(EntityService), nameof(Process), nameof(SpawnEntity), data.VhId);

            if (this.TryGetId(data.VhId, out EntityId id) == true)
            {
                ref EntityStatus status = ref this.QueryById<EntityStatus>(id);
                status.Increment(EntityModificationTypeEnum.Spawned);

                return;
            }

            // Enqueue SoftSpawn entity event
            // This is enqueued before HardSpawn is published in case the initializer
            // Spawns any other entities. This ensture a first in first out order of
            // SoftSpawn events

            this.Simulation.Enqueue(new EventDto()
            {
                SourceId = NameSpace<EntityService>.Instance.Create(eventId),
                Data = new SoftSpawnEntity()
                {
                    VhId = data.VhId
                }
            });

            // Publish HardSPawn even immidiately
            this.Simulation.Publish(new EventDto()
            {
                SourceId = NameSpace<EntityService>.Instance.Create(eventId),
                Data = new HardSpawnEntity<EntityInitializerDelegate>()
                {
                    VhId = data.VhId,
                    Type = data.Type,
                    Initializer = data.Initializer
                }
            });
        }

        public void Process(VhId eventId, HardSpawnEntity data)
        {
            ref EntityId id = ref this.GetOrAddId(data.VhId, out bool exists);
            if (exists == true)
            { // Unable to hard spawn - entity already exists
                throw new NotImplementedException();
            }

            EntityInitializer initializer = _entityTypeService.Value.GetProviderByType(data.Type).HardSpawnInstanceEntity(eventId, data.VhId, out id);
        }

        public void Process(VhId eventId, HardSpawnEntity<EntityInitializerDelegate> data)
        {
            ref EntityId id = ref this.GetOrAddId(data.VhId, out bool exists);
            if (exists == true)
            { // Unable to hard spawn - entity already exists
                throw new NotImplementedException();
            }

            EntityInitializer initializer = _entityTypeService.Value.GetProviderByType(data.Type).HardSpawnInstanceEntity(eventId, data.VhId, out id);
            data.Initializer.Invoke(this, data.Type, id, ref initializer);
        }

        public void Process(VhId eventId, SoftSpawnEntity data)
        {
            if (this.TryGetId(data.VhId, out EntityId id) == false)
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Unable to soft spawn entity, unknown VhId {VhId}", nameof(EntityService), nameof(Process), nameof(SoftSpawnEntity), data.VhId);
                return;
            }

            ref EntityStatus status = ref this.QueryById<EntityStatus>(id, out GroupIndex groupIndex, out bool exists);
            if (exists == false || status.Value != EntityStatusEnum.HardSpawned)
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Id = {Id}, Exists = {Exists}, Status = {Status}", nameof(EntityService), nameof(Process), nameof(SoftSpawnEntity), id.VhId, exists, exists ? status.Value : null);
                return;
            }


            Id<IEntityType> typeId = this.QueryByGroupIndex<InstanceEntity>(in groupIndex).TypeId;
            _entityTypeService.Value.GetProviderByTypeId(typeId).SoftSpawnInstanceEntity(in eventId, in id, in groupIndex, ref status);
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
            _logger.Verbose("{ClassName}::{MethodName}<{GenericType}> - EntityVhId = {EntityVhId}", nameof(EntityService), nameof(InternalRevert), nameof(SpawnEntity), data.VhId);

            if (this.TryGetId(data.VhId, out EntityId id) == false)
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Unable to soft spawn entity, unknown VhId {VhId}", nameof(EntityService), nameof(InternalRevert), nameof(SpawnEntity), data.VhId);
                return;
            }

            ref EntityStatus status = ref this.QueryById<EntityStatus>(id, out _, out bool exists);
            if (exists == false)
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Id = {Id}, Exists = {Exists}, Status = {Status}", nameof(EntityService), nameof(InternalRevert), nameof(SpawnEntity), id.VhId, exists, exists ? status.Value : null);
                return;
            }

            int spawnCount = 0;
            if ((spawnCount = status.Increment(EntityModificationTypeEnum.Despawned)) == 0)
            {
                this.Simulation.Enqueue(new EventDto()
                {
                    SourceId = NameSpace<EntityService>.Instance.Create(eventId),
                    Data = new SoftDespawnEntity()
                    {
                        VhId = data.VhId
                    }
                });
            }
            else
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Id = {Id}, Exists = {Exists}, Status = {Status}, SpawnCount = {SpawnCount}", nameof(EntityService), nameof(InternalRevert), nameof(SpawnEntity), id.VhId, exists, exists ? status.Value : null, spawnCount);
            }

            // TODO: Investigate why the HardDespawn event is published despite the SoftDespawn being locked behind the Despawn counter
            // I dont remember if this was by design or if its just a bug
            this.Simulation.Enqueue(new EventDto()
            {
                SourceId = NameSpace<EntityService>.Instance.Create(eventId),
                Data = new HardDespawnEntity()
                {
                    IsPrivate = true,
                    IsPredictable = true,
                    VhId = data.VhId
                }
            });

            status.Value = EntityStatusEnum.RevertSpawnEnqueued;
        }

        public void Process(VhId eventId, DespawnEntity data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{GenericType}> - EntityVhId = {EntityVhId}", nameof(EntityService), nameof(Process), nameof(DespawnEntity), data.VhId);

            if (this.TryGetId(data.VhId, out EntityId id) == false)
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Unable to despawn entity, unknown VhId {VhId}", nameof(EntityService), nameof(Process), nameof(DespawnEntity), data.VhId);
                return;
            }

            ref EntityStatus status = ref this.QueryById<EntityStatus>(id, out GroupIndex groupIndex, out bool exists);
            int spawnedCount = 0;
            if (exists == false || (spawnedCount = status.Increment(EntityModificationTypeEnum.Despawned)) != 0)
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Id = {Id}, Exists = {Exists}, Status = {Status}, SpawnedCount = {SpawnedCount}", nameof(EntityService), nameof(Process), nameof(DespawnEntity), id.VhId, exists, exists ? status.Value : null, spawnedCount);
                return;
            }


            this.Simulation.Enqueue(new EventDto()
            {
                SourceId = NameSpace<EntityService>.Instance.Create(eventId),
                Data = new SoftDespawnEntity()
                {
                    VhId = data.VhId
                }
            });

            this.Simulation.Enqueue(new EventDto()
            {
                SourceId = NameSpace<EntityService>.Instance.Create(eventId),
                Data = new HardDespawnEntity()
                {
                    IsPrivate = false,
                    IsPredictable = false,
                    VhId = data.VhId
                }
            });

            status.Value = EntityStatusEnum.SoftDespawnEnqueued;
        }

        public void Process(VhId eventId, SoftDespawnEntity data)
        {
            if (this.TryGetId(data.VhId, out EntityId id) == false)
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Unable to soft despawn entity. VhId = {VhId}", nameof(EntityService), nameof(Process), nameof(SoftDespawnEntity), data.VhId);
                return;
            }

            ref EntityStatus status = ref this.QueryById<EntityStatus>(id, out GroupIndex groupIndex, out bool exists);
            if (exists == false || status.Value != EntityStatusEnum.SoftDespawnEnqueued)
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Unable to soft despawn entity. Id = {Id}, Exists = {Exists}, Status = {Status}", nameof(EntityService), nameof(Process), nameof(SoftDespawnEntity), id.VhId, exists, exists ? status.Value : null);
                return;
            }

            Id<IEntityType> typeId = this.QueryByGroupIndex<InstanceEntity>(in groupIndex).TypeId;
            _entityTypeService.Value.GetProviderByTypeId(typeId).SoftDespawnInstanceEntity(in eventId, in id, in groupIndex, ref status);
            status.Value = EntityStatusEnum.SoftDespawned;
        }

        public void Process(VhId eventId, HardDespawnEntity data)
        {
            if (this.TryGetId(data.VhId, out EntityId id) == false)
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Unable to hard despawn entity. VhId = {VhId}", nameof(EntityService), nameof(Process), nameof(HardDespawnEntity), data.VhId);
                return;
            }

            ref EntityStatus status = ref this.QueryById<EntityStatus>(id, out GroupIndex groupIndex, out bool exists);

            if (exists == false)
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Unable to hard despawn entity. Id = {Id}, Exists = {Exists}, Status = {Status}", nameof(EntityService), nameof(Process), nameof(HardDespawnEntity), id.VhId, exists, exists ? status.Value : null);
                return;
            }

            Id<IEntityType> typeId = this.QueryByGroupIndex<InstanceEntity>(in groupIndex).TypeId;
            IEntityTypeProvider descriptorEngine = _entityTypeService.Value.GetProviderByTypeId(typeId);

            if (status.Value < EntityStatusEnum.SoftDespawned)
            { // Ensure an entity gets soft despawned if it hasn't been already
                descriptorEngine.SoftDespawnInstanceEntity(in eventId, in id, in groupIndex, ref status);
                status.Value = EntityStatusEnum.SoftDespawned;
            }

            descriptorEngine.HardDespawnInstanceEntity(in eventId, in id, in groupIndex, ref status);
            this.RemoveId(id);
            status.Value = EntityStatusEnum.HardDespawned;
        }

        public void Revert(VhId eventId, DespawnEntity data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{GenericType}> - EntityVhId = {EntityVhId}", nameof(EntityService), nameof(Revert), nameof(DespawnEntity), data.VhId);

            if (this.TryGetId(data.VhId, out EntityId id) == false)
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Unable to revert despawn entity, unknown VhId {VhId}, Id not found.", nameof(EntityService), nameof(Revert), nameof(DespawnEntity), data.VhId);
                return;
            }

            ref EntityStatus status = ref this.QueryById<EntityStatus>(id, out GroupIndex groupIndex, out bool exists);

            int spawnedCount = 0;
            if (exists == false || (spawnedCount = status.Increment(EntityModificationTypeEnum.Spawned)) != 1)
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Id = {Id}, Exists = {Exists}, Status = {Status}, SpawnedCount = {SpawnedCount}", nameof(EntityService), nameof(Revert), nameof(DespawnEntity), id.VhId, exists, exists ? status.Value : null, spawnedCount);
                return;
            }

            this.Simulation.Enqueue(new EventDto()
            {
                SourceId = NameSpace<EntityService>.Instance.Create(eventId),
                Data = new SoftSpawnEntity()
                {
                    VhId = data.VhId
                }
            });

            status.Value = EntityStatusEnum.HardSpawned;
        }
    }
}
