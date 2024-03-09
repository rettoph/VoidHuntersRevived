using Svelto.ECS;
using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Domain.Entities.Events;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal partial class EntityService :
        IEventEngine<SpawnEntity>,
        IEventEngine<HardSpawnEntity>,
        IEventEngine<SoftSpawnEntity>,
        IRevertEventEngine<SpawnEntity>,
        IEventEngine<DespawnEntity>,
        IRevertEventEngine<DespawnEntity>,
        IEventEngine<SoftDespawnEntity>,
        IEventEngine<EnqueueHardDespawn>,
        IEventEngine<HardDespawnEntity>
    {
        public EntityId Spawn(VhId sourceId, IEntityType type, VhId vhid, Id<ITeam> teamId, InstanceEntityInitializerDelegate? initializer)
        {
            this.Simulation.Publish(NameSpace<EntityService>.Instance.Create(sourceId), new SpawnEntity()
            {
                Type = type,
                VhId = vhid,
                TeamId = teamId,
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

            if (this.TryGetId(data.VhId, out EntityId id) == false)
            {
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
                        TeamId = data.TeamId,
                        Type = data.Type,
                        Initializer = data.Initializer
                    }
                });
            }
            else
            {
                ref EntityStatus status = ref this.QueryById<EntityStatus>(id);
                status.Increment(EntityModificationTypeEnum.Spawned);
            }
        }

        public void Process(VhId eventId, HardSpawnEntity data)
        {
            ref EntityId id = ref this.GetOrAddId(data.VhId, out bool exists);
            if (exists == false)
            {
                EntityInitializer initializer = this.GetDescriptorEngine(data.Type.Descriptor.Id).HardSpawn(eventId, data.VhId, data.TeamId, out id);
                initializer.Init(new EntityStatus(EntityStatusEnum.HardSpawned));
                _entityTypeInitializer.Get(data.Type).InitializeInstance(this, ref initializer, in id);
                data.Initializer?.Invoke(this, ref initializer, in id);
            }
            else
            {

            }
        }

        public void Process(VhId eventId, SoftSpawnEntity data)
        {
            if (this.TryGetId(data.VhId, out EntityId id))
            {
                ref EntityStatus status = ref this.QueryById<EntityStatus>(id, out GroupIndex groupIndex, out bool exists);

                if (exists && status.Value == EntityStatusEnum.HardSpawned)
                {
                    Id<VoidHuntersEntityDescriptor> descriptorId = this.QueryByGroupIndex<Id<VoidHuntersEntityDescriptor>>(in groupIndex);
                    this.GetDescriptorEngine(descriptorId).SoftSpawn(in eventId, in id, in groupIndex, ref status);
                    status.Value = EntityStatusEnum.SoftSpawned;
                }
                else
                {
                    _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Id = {Id}, Exists = {Exists}, Status = {Status}", nameof(EntityService), nameof(Process), nameof(SoftSpawnEntity), id.VhId, exists, exists ? status.Value : null);
                }
            }
            else
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Unable to soft spawn entity, unknown VhId {VhId}", nameof(EntityService), nameof(Process), nameof(SoftSpawnEntity), data.VhId);
            }
        }

        public void Revert(VhId eventId, SpawnEntity data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{GenericType}> - EntityVhId = {EntityVhId}", nameof(EntityService), nameof(Revert), nameof(SpawnEntity), data.VhId);

            if (this.TryGetId(data.VhId, out EntityId id))
            {
                ref EntityStatus status = ref this.QueryById<EntityStatus>(id, out _, out bool exists);

                if (exists)
                {
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
                        _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Id = {Id}, Exists = {Exists}, Status = {Status}, SpawnCount = {SpawnCount}", nameof(EntityService), nameof(Revert), nameof(SpawnEntity), id.VhId, exists, exists ? status.Value : null, spawnCount);
                    }

                    this.Simulation.Enqueue(new EventDto()
                    {
                        SourceId = NameSpace<EntityService>.Instance.Create(eventId),
                        Data = new HardDespawnEntity()
                        {
                            VhId = data.VhId
                        }
                    });

                    status.Value = EntityStatusEnum.RevertSpawnEnqueued;
                }
                else
                {
                    _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Id = {Id}, Exists = {Exists}, Status = {Status}", nameof(EntityService), nameof(Revert), nameof(SpawnEntity), id.VhId, exists, exists ? status.Value : null);
                }
            }
            else
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Unable to soft spawn entity, unknown VhId {VhId}", nameof(EntityService), nameof(Revert), nameof(SpawnEntity), data.VhId);
            }
        }

        public void Process(VhId eventId, DespawnEntity data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{GenericType}> - EntityVhId = {EntityVhId}", nameof(EntityService), nameof(Process), nameof(DespawnEntity), data.VhId);

            if (this.TryGetId(data.VhId, out EntityId id))
            {
                ref EntityStatus status = ref this.QueryById<EntityStatus>(id, out GroupIndex groupIndex, out bool exists);

                int spawnedCount = 0;
                if (exists && (spawnedCount = status.Increment(EntityModificationTypeEnum.Despawned)) == 0)
                {
                    this.Simulation.Enqueue(new EventDto()
                    {
                        SourceId = NameSpace<EntityService>.Instance.Create(eventId),
                        Data = new SoftDespawnEntity()
                        {
                            VhId = data.VhId
                        }
                    });

                    this.Simulation.Publish(new EventDto()
                    {
                        SourceId = NameSpace<EntityService>.Instance.Create(eventId),
                        Data = new EnqueueHardDespawn()
                        {
                            VhId = data.VhId
                        }
                    });

                    status.Value = EntityStatusEnum.SoftDespawnEnqueued;
                }
                else
                {
                    _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Id = {Id}, Exists = {Exists}, Status = {Status}, SpawnedCount = {SpawnedCount}", nameof(EntityService), nameof(Process), nameof(DespawnEntity), id.VhId, exists, exists ? status.Value : null, spawnedCount);
                }
            }
            else
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - U00nable to despawn entity, unknown VhId {VhId}", nameof(EntityService), nameof(Process), nameof(DespawnEntity), data.VhId);
            }
        }

        public void Process(VhId eventId, SoftDespawnEntity data)
        {
            if (this.TryGetId(data.VhId, out EntityId id) == true)
            {
                ref EntityStatus status = ref this.QueryById<EntityStatus>(id, out GroupIndex groupIndex, out bool exists);

                if (exists && status.Value == EntityStatusEnum.SoftDespawnEnqueued)
                {
                    Id<VoidHuntersEntityDescriptor> descriptorId = this.QueryByGroupIndex<Id<VoidHuntersEntityDescriptor>>(in groupIndex);
                    this.GetDescriptorEngine(descriptorId).SoftDespawn(in eventId, in id, in groupIndex, ref status);
                    status.Value = EntityStatusEnum.SoftDespawned;
                }
                else
                {
                    _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Unable to soft despawn entity. Id = {Id}, Exists = {Exists}, Status = {Status}", nameof(EntityService), nameof(Process), nameof(SoftDespawnEntity), id.VhId, exists, exists ? status.Value : null);
                }
            }
            else
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Unable to soft despawn entity. VhId = {VhId}", nameof(EntityService), nameof(Process), nameof(SoftDespawnEntity), data.VhId);
            }
        }

        public void Process(VhId eventId, EnqueueHardDespawn data)
        {
            this.Simulation.Enqueue(new EventDto()
            {
                SourceId = NameSpace<EntityService>.Instance.Create(eventId),
                Data = new HardDespawnEntity()
                {
                    VhId = data.VhId
                }
            });
        }

        public void Process(VhId eventId, HardDespawnEntity data)
        {
            if (this.TryGetId(data.VhId, out EntityId id))
            {
                ref EntityStatus status = ref this.QueryById<EntityStatus>(id, out GroupIndex groupIndex, out bool exists);

                if (exists)
                {
                    Id<VoidHuntersEntityDescriptor> descriptorId = this.QueryByGroupIndex<Id<VoidHuntersEntityDescriptor>>(in groupIndex);
                    IVoidHuntersEntityDescriptorEngine descriptorEngine = this.GetDescriptorEngine(descriptorId);

                    if (status.Value < EntityStatusEnum.SoftDespawned)
                    {
                        descriptorEngine.SoftDespawn(in eventId, in id, in groupIndex, ref status);
                        status.Value = EntityStatusEnum.SoftDespawned;
                    }

                    descriptorEngine.HardDespawn(in eventId, in id, in groupIndex, ref status);
                    this.RemoveId(id);
                    status.Value = EntityStatusEnum.HardDespawned;
                }
                else
                {
                    _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Unable to hard despawn entity. Id = {Id}, Exists = {Exists}, Status = {Status}", nameof(EntityService), nameof(Process), nameof(HardDespawnEntity), id.VhId, exists, exists ? status.Value : null);
                }
            }
            else
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Unable to hard despawn entity. VhId = {VhId}", nameof(EntityService), nameof(Process), nameof(HardDespawnEntity), data.VhId);
            }
        }

        public void Revert(VhId eventId, DespawnEntity data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{GenericType}> - EntityVhId = {EntityVhId}", nameof(EntityService), nameof(Revert), nameof(DespawnEntity), data.VhId);

            if (this.TryGetId(data.VhId, out EntityId id))
            {
                ref EntityStatus status = ref this.QueryById<EntityStatus>(id, out GroupIndex groupIndex, out bool exists);

                int spawnedCount = 0;
                if (exists && (spawnedCount = status.Increment(EntityModificationTypeEnum.Spawned)) == 1)
                {
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
                else
                {
                    _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Id = {Id}, Exists = {Exists}, Status = {Status}, SpawnedCount = {SpawnedCount}", nameof(EntityService), nameof(Revert), nameof(DespawnEntity), id.VhId, exists, exists ? status.Value : null, spawnedCount);
                }
            }
            else
            {
                _logger.Warning("{ClassName}::{MethdName}<{GenericType}> - Unable to revert despawn entity, unknown VhId {VhId}, Id not found.", nameof(EntityService), nameof(Revert), nameof(DespawnEntity), data.VhId);
            }
        }
    }
}
