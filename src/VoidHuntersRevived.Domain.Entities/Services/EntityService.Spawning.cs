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

            while (_added.TryDequeue(out EntityId id))
            {
                EntityStatus status = this.QueryById<EntityStatus>(id, out GroupIndex groupIndex);

                if(status.IsDespawned == false)
                {
                    _logger.Verbose("{ClassName}<{GenericType}>::{MethodName} - Soft spawning entity {EntityId}", nameof(EntityService), nameof(Flush), id.VhId);

                    Id<VoidHuntersEntityDescriptor> descriptorId = this.QueryByGroupIndex<Id<VoidHuntersEntityDescriptor>>(in groupIndex);
                    this.GetDescriptorEngine(descriptorId).SoftSpawn(in id, groupIndex);
                }
                else
                {
                    _logger.Warning("{ClassName}::{MethodName} - Unable to SoftSpawn {EntityId}, IsDespawned = {IsDespawned}", nameof(EntityService), nameof(Flush), id.VhId, status.IsDespawned);
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

        public void Process(VhId eventId, SpawnEntity data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Attempting to spawn Entity {Id}, {Type}", nameof(EntityService), nameof(Process), nameof(Events.SpawnEntity), data.VhId.Value, data.Type.Key);

            this.SpawnEntity(data.Type, data.VhId, data.TeamId, data.Initializer);
        }

        public void Revert(VhId eventId, SpawnEntity data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Attempting to revert spawn Entity {Id}, {Type}", nameof(EntityService), nameof(Revert), nameof(Events.SpawnEntity), data.VhId.Value, data.Type.Key);

            EntityId id = this.GetId(data.VhId);
            this.SoftDespawnEntity(id);
            this.HardDespawnEntity(id);
        }

        public void Process(VhId eventId, SoftDespawnEntity data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Attempting to soft despawn Entity {Id}", nameof(Services.EntityService), nameof(Process), nameof(Events.SoftDespawnEntity), data.VhId.Value);

            EntityId id = this.GetId(data.VhId);
            this.SoftDespawnEntity(id);
        }

        public void Revert(VhId eventId, SoftDespawnEntity data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Attempting to revert soft despawn Entity {Id}", nameof(EntityService), nameof(Revert), nameof(Events.SoftDespawnEntity), data.VhId.Value);

            if(this.TryGetId(data.VhId, out EntityId id))
            {
                this.RevertSoftDespawnEntity(id);
            }
        }

        public void Process(VhId eventId, HardDespawnEntity data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Attempting to hard despawn Entity {Id}", nameof(Services.EntityService), nameof(Process), nameof(Events.HardDespawnEntity), data.VhId.Value);

            EntityId id = this.GetId(data.VhId);
            this.HardDespawnEntity(id);
        }

        public void Revert(VhId eventId, HardDespawnEntity data)
        {
            throw new NotImplementedException();
        }

        private EntityId SpawnEntity(IEntityType type, in VhId vhid, in Id<ITeam> teamId, EntityInitializerDelegate? initializerDelegate)
        {
            EntityInitializer initializer = this.GetDescriptorEngine(type.Descriptor.Id).HardSpawn(vhid, teamId, out EntityId id);
            this.Add(id);

            _types.GetConfiguration(type).Initialize(this, ref initializer, in id);

            initializerDelegate?.Invoke(this, ref initializer, in id);

            return id;
        }

        private void SoftDespawnEntity(EntityId id)
        {
            Id<VoidHuntersEntityDescriptor> descriptorId = this.QueryById<Id<VoidHuntersEntityDescriptor>>(id, out GroupIndex groupIndex);
            this.GetDescriptorEngine(descriptorId).SoftDespawn(in id, groupIndex);
        }

        private void RevertSoftDespawnEntity(EntityId id)
        {
            Id<VoidHuntersEntityDescriptor> descriptorId = this.QueryById<Id<VoidHuntersEntityDescriptor>>(id, out GroupIndex groupIndex);
            this.GetDescriptorEngine(descriptorId).RevertSoftDespawn(in id, groupIndex);
        }

        private void HardDespawnEntity(EntityId id)
        {
            Id<VoidHuntersEntityDescriptor> descriptorId = this.QueryById<Id<VoidHuntersEntityDescriptor>>(id, out GroupIndex groupIndex);
            this.GetDescriptorEngine(descriptorId).HardDespawn(in id, groupIndex);

            this.Remove(id);
        }
    }
}
