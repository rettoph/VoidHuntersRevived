using Guppy.Common.Collections;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Messages;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Domain.Entities.Events;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Domain.Entities.Engines;

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
        public EntityId Spawn(IEntityType type, VhId vhid, Id<ITeam> teamId, EntityInitializerDelegate? initializer)
        {
            _logger.Verbose("{ClassName}::{MethodName} - EntityVhId = {EntityVhId}", nameof(EntityService), nameof(Spawn), vhid);

            this.Simulation.Publish(NameSpace<EntityService>.Instance, new SpawnEntity()
            {
                Type = type,
                VhId = vhid,
                TeamId = teamId,
                Initializer = initializer
            });

            return this.GetId(vhid);
        }

        public void Despawn(VhId vhid)
        {
            _logger.Verbose("{ClassName}::{MethodName} - EntityVhId = {EntityVhId}", nameof(EntityService), nameof(Despawn), vhid);
            this.Simulation.Publish(NameSpace<EntityService>.Instance, new SoftDespawnEntity()
            {
                VhId = vhid
            });

            this.Simulation.Publish(NameSpace<EntityService>.Instance, new HardDespawnEntity()
            {
                VhId = vhid
            });
        }

        public void Flush()
        {
            _scheduler.SubmitEntities();

            while (_modifications.TryDequeue(out EntityModificationRequest request))
            {
                _logger.Verbose("{ClassName}::{MethodName} - EntityModificationType = {EntityModificationType}, EntityId = {EntityId}", nameof(EntityService), nameof(Flush), request.ModificationType, request.Id.VhId);

                switch(request.ModificationType)
                {
                    case EntityModificationType.SoftSpawn:
                        this.SoftSpawnEntity(in request.Id);
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

        public void Despawn(EntityId id)
        {
            this.Despawn(id.VhId);
        }

        public bool IsSpawned(EntityId id)
        {
            if(this.TryQueryById<EntityStatus>(id, out EntityStatus status))
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
            this.SpawnEntity(data.Type, data.VhId, data.TeamId, data.Initializer);
        }

        public void Revert(VhId eventId, SpawnEntity data)
        {
            EntityId id = this.GetId(data.VhId);

            this.EnqueuEntityModification(new EntityModificationRequest(EntityModificationType.SoftDespawn, id));
            this.EnqueuEntityModification(new EntityModificationRequest(EntityModificationType.HardDespawn, id));

            ref var status = ref this.QueryById<EntityStatus>(id);
            status.Value = EntityStatusEnum.SoftDespawnEnqueued;
        }

        public void Process(VhId eventId, SoftDespawnEntity data)
        {
            EntityId id = this.GetId(data.VhId);
            this.EnqueuEntityModification(new EntityModificationRequest(EntityModificationType.SoftDespawn, id));

            ref var status = ref this.QueryById<EntityStatus>(id);
            status.Value = EntityStatusEnum.SoftDespawnEnqueued;
        }

        public void Revert(VhId eventId, SoftDespawnEntity data)
        {
            if(this.TryGetId(data.VhId, out EntityId id))
            {
                this.EnqueuEntityModification(new EntityModificationRequest(EntityModificationType.RevertSoftDespawn, id));
            }
            else
            {
                _logger.Warning("{ClassName}::{MethodName}<{EventName}> - Unable to revert soft despawn Entity {Id}.", nameof(EntityService), nameof(Revert), nameof(Events.SoftDespawnEntity), data.VhId.Value);
            }
        }

        public void Process(VhId eventId, HardDespawnEntity data)
        {
            EntityId id = this.GetId(data.VhId);
            this.EnqueuEntityModification(new EntityModificationRequest(EntityModificationType.HardDespawn, id));
        }

        public void Revert(VhId eventId, HardDespawnEntity data)
        {
            throw new NotImplementedException();
        }

        private EntityId SpawnEntity(IEntityType type, in VhId vhid, in Id<ITeam> teamId, EntityInitializerDelegate? initializerDelegate)
        {
            EntityInitializer initializer = this.GetDescriptorEngine(type.Descriptor.Id).HardSpawn(vhid, teamId, out EntityId id);
            this.AddId(id);
            this.EnqueuEntityModification(new EntityModificationRequest(EntityModificationType.SoftSpawn, id));

            _types.GetConfiguration(type).Initialize(this, ref initializer, in id);

            initializerDelegate?.Invoke(this, ref initializer, in id);

            return id;
        }

        private void SoftSpawnEntity(in EntityId id)
        {
            ref EntityStatus status = ref this.QueryById<EntityStatus>(id, out GroupIndex groupIndex, out bool exists);

            if (exists && status.Value == EntityStatusEnum.NotSpawned)
            {
                Id<VoidHuntersEntityDescriptor> descriptorId = this.QueryByGroupIndex<Id<VoidHuntersEntityDescriptor>>(in groupIndex);
                this.GetDescriptorEngine(descriptorId).SoftSpawn(in id, in groupIndex, ref status);
            }
            else
            {
                _logger.Warning("{ClassName}::{MethdName} - Id = {Id}, Exists = {Exists}, Status = {Status}", nameof(EntityService), nameof(SoftSpawnEntity), id.VhId, exists, exists ? status.Value : null);
            }
        }

        private void SoftDespawnEntity(in EntityModificationRequest request)
        {
            ref EntityStatus status = ref this.QueryById<EntityStatus>(request.Id, out GroupIndex groupIndex, out bool exists);

            if(exists && status.Value == EntityStatusEnum.SoftDespawnEnqueued)
            {
                Id<VoidHuntersEntityDescriptor> descriptorId = this.QueryByGroupIndex<Id<VoidHuntersEntityDescriptor>>(in groupIndex);
                this.GetDescriptorEngine(descriptorId).SoftDespawn(in request.Id, in groupIndex, ref status);
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
                this.GetDescriptorEngine(descriptorId).RevertSoftDespawn(in request.Id, in groupIndex, ref status);
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
                this.GetDescriptorEngine(descriptorId).HardDespawn(in request.Id, in groupIndex, ref status);
                this.RemoveId(request.Id);
            }
            else
            {
                _logger.Warning("{ClassName}::{MethdName} - Id = {Id}, Exists = {Exists}, Status = {Status}", nameof(EntityService), nameof(HardDespawnEntity), request.Id.VhId, exists, exists ? status.Value : null);
            }
        }
    }
}
