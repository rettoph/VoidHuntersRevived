using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Domain.Entities.Events;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal partial class EntityService :
        IEventEngine<SpawnEntity>,
        IRevertEventEngine<SpawnEntity>,
        IEventEngine<HardDespawnEntity>,
        IRevertEventEngine<HardDespawnEntity>,
        IEventEngine<SoftDespawnEntity>,
        IRevertEventEngine<SoftDespawnEntity>
    {
        public EntityId Spawn(VhId sourceId, IEntityType type, VhId vhid, Id<ITeam> teamId, EntityInitializerDelegate? initializer)
        {
            _logger.Verbose("{ClassName}::{MethodName} - EntityVhId = {EntityVhId}", nameof(EntityService), nameof(Spawn), vhid);

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
            this.Simulation.Publish(NameSpace<EntityService>.Instance.Create(sourceId), new SoftDespawnEntity()
            {
                VhId = vhid
            });

            this.Simulation.Publish(NameSpace<EntityService>.Instance.Create(sourceId), new HardDespawnEntity()
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

            while (_modifications.TryDequeue(out EntityModificationRequest request))
            {
                _logger.Verbose("{ClassName}::{MethodName} - EntityModificationType = {EntityModificationType}, EntityId = {EntityId}", nameof(EntityService), nameof(Flush), request.ModificationType, request.Id.VhId);

                switch (request.ModificationType)
                {
                    case EntityModificationType.SoftSpawn:
                        this.SoftSpawnEntity(in request.SourceEventId, in request.Id);
                        break;
                    case EntityModificationType.SoftDespawn:
                        this.SoftDespawnEntity(in request);
                        break;
                    case EntityModificationType.HardDespawn:
                        this.HardDespawnEntity(in request);
                        break;
                    case EntityModificationType.RevertSoftDespawn:
                        this.RevertSoftDespawnEntity(in request);
                        break;
                }
            }
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
            this.SpawnEntity(eventId, data.Type, data.VhId, data.TeamId, data.Initializer);
        }

        public void Revert(VhId eventId, SpawnEntity data)
        {
            if (this.TryGetId(data.VhId, out EntityId id))
            {
                this.EnqueuEntityModification(new EntityModificationRequest(eventId, EntityModificationType.SoftDespawn, id));
                this.EnqueuEntityModification(new EntityModificationRequest(eventId, EntityModificationType.HardDespawn, id));

                ref var status = ref this.QueryById<EntityStatus>(id);
                status.Value = EntityStatusEnum.SoftDespawnEnqueued;
            }
        }

        public void Process(VhId eventId, SoftDespawnEntity data)
        {
            if (this.TryGetId(data.VhId, out EntityId id))
            {
                this.EnqueuEntityModification(new EntityModificationRequest(eventId, EntityModificationType.SoftDespawn, id));

                ref var status = ref this.QueryById<EntityStatus>(id);
                status.Value = EntityStatusEnum.SoftDespawnEnqueued;
            }
        }

        public void Revert(VhId eventId, SoftDespawnEntity data)
        {
            if (this.TryGetId(data.VhId, out EntityId id))
            {
                this.EnqueuEntityModification(new EntityModificationRequest(eventId, EntityModificationType.RevertSoftDespawn, id));
            }
        }

        public void Process(VhId eventId, HardDespawnEntity data)
        {
            if (this.TryGetId(data.VhId, out EntityId id))
            {
                this.EnqueuEntityModification(new EntityModificationRequest(eventId, EntityModificationType.HardDespawn, id));
            }
        }

        public void Revert(VhId eventId, HardDespawnEntity data)
        {
            throw new NotImplementedException();
        }

        private EntityId SpawnEntity(VhId sourceEventId, IEntityType type, in VhId vhid, in Id<ITeam> teamId, EntityInitializerDelegate? initializerDelegate)
        {
            EntityInitializer initializer = this.GetDescriptorEngine(type.Descriptor.Id).HardSpawn(sourceEventId, vhid, teamId, out EntityId id);
            this.AddId(id);
            this.EnqueuEntityModification(new EntityModificationRequest(sourceEventId, EntityModificationType.SoftSpawn, id));

            _types.GetConfiguration(type).Initialize(this, ref initializer, in id);

            initializerDelegate?.Invoke(this, ref initializer, in id);

            return id;
        }

        private void SoftSpawnEntity(in VhId sourceEventId, in EntityId id)
        {
            ref EntityStatus status = ref this.QueryById<EntityStatus>(id, out GroupIndex groupIndex, out bool exists);

            if (exists && status.Value == EntityStatusEnum.NotSpawned)
            {
                Id<VoidHuntersEntityDescriptor> descriptorId = this.QueryByGroupIndex<Id<VoidHuntersEntityDescriptor>>(in groupIndex);
                this.GetDescriptorEngine(descriptorId).SoftSpawn(in sourceEventId, in id, in groupIndex, ref status);
            }
            else
            {
                _logger.Warning("{ClassName}::{MethdName} - Id = {Id}, Exists = {Exists}, Status = {Status}", nameof(EntityService), nameof(SoftSpawnEntity), id.VhId, exists, exists ? status.Value : null);
            }
        }

        private void SoftDespawnEntity(in EntityModificationRequest request)
        {
            ref EntityStatus status = ref this.QueryById<EntityStatus>(request.Id, out GroupIndex groupIndex, out bool exists);

            if (exists && status.Value == EntityStatusEnum.SoftDespawnEnqueued)
            {
                Id<VoidHuntersEntityDescriptor> descriptorId = this.QueryByGroupIndex<Id<VoidHuntersEntityDescriptor>>(in groupIndex);
                this.GetDescriptorEngine(descriptorId).SoftDespawn(in request.SourceEventId, in request.Id, in groupIndex, ref status);
            }
            else
            {
                _logger.Warning("{ClassName}::{MethdName} - Id = {Id}, Exists = {Exists}, Status = {Status}", nameof(EntityService), nameof(SoftDespawnEntity), request.Id.VhId, exists, exists ? status.Value : null);
            }
        }

        private void RevertSoftDespawnEntity(in EntityModificationRequest request)
        {
            ref EntityStatus status = ref this.QueryById<EntityStatus>(request.Id, out GroupIndex groupIndex, out bool exists);

            if (exists && status.Value == EntityStatusEnum.SoftDespawned)
            {
                Id<VoidHuntersEntityDescriptor> descriptorId = this.QueryByGroupIndex<Id<VoidHuntersEntityDescriptor>>(in groupIndex);
                this.GetDescriptorEngine(descriptorId).RevertSoftDespawn(in request.SourceEventId, in request.Id, in groupIndex, ref status);
            }
            else
            {
                _logger.Warning("{ClassName}::{MethdName} - Id = {Id}, Exists = {Exists}, Status = {Status}", nameof(EntityService), nameof(RevertSoftDespawnEntity), request.Id.VhId, exists, exists ? status.Value : null);
            }
        }

        private void HardDespawnEntity(in EntityModificationRequest request)
        {
            ref EntityStatus status = ref this.QueryById<EntityStatus>(request.Id, out GroupIndex groupIndex, out bool exists);

            if (exists && status.Value == EntityStatusEnum.SoftDespawned)
            {
                Id<VoidHuntersEntityDescriptor> descriptorId = this.QueryByGroupIndex<Id<VoidHuntersEntityDescriptor>>(in groupIndex);
                this.GetDescriptorEngine(descriptorId).HardDespawn(in request.SourceEventId, in request.Id, in groupIndex, ref status);
                this.RemoveId(request.Id);
            }
            else
            {
                _logger.Warning("{ClassName}::{MethdName} - Id = {Id}, Exists = {Exists}, Status = {Status}", nameof(EntityService), nameof(HardDespawnEntity), request.Id.VhId, exists, exists ? status.Value : null);
            }
        }
    }
}
