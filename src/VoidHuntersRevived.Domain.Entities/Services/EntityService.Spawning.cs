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
        IEventEngine<SpawnEntityDescriptor>,
        IRevertEventEngine<SpawnEntityDescriptor>,
        IEventEngine<SpawnEntityType>,
        IRevertEventEngine<SpawnEntityType>,
        IEventEngine<HardDespawnEntity>,
        IRevertEventEngine<HardDespawnEntity>,
        IEventEngine<SoftDespawnEntity>,
        IRevertEventEngine<SoftDespawnEntity>
    {
        public EntityId Spawn(IEntityType type, VhId vhid, TeamId teamId, EntityInitializerDelegate? initializer)
        {
            this.Simulation.Publish(NameSpace<EntityService>.Instance, new SpawnEntityType()
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

        public void Process(VhId eventId, SpawnEntityDescriptor data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Attempting to spawn EntityDescriptor {Id}, {Descriptor}", nameof(EntityService), nameof(Process), nameof(SpawnEntityDescriptor), data.VhId.Value, data.DescriptorId.Value);

            this.SpawnDescriptor(data.DescriptorId, data.VhId, data.TeamId, data.Initializer);
        }

        public void Revert(VhId eventId, SpawnEntityDescriptor data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Attempting to revert spawn EntityDescriptor {Id}, {Descriptor}", nameof(EntityService), nameof(Revert), nameof(SpawnEntityDescriptor), data.VhId.Value, data.DescriptorId.Value);

            EntityId id = this.GetId(data.VhId);
            this.SoftDespawnEntity(id);
            this.HardDespawnEntity(id);
        }

        public void Process(VhId eventId, SpawnEntityType data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Attempting to spawn EntityType {Id}, {Type}", nameof(EntityService), nameof(Process), nameof(SpawnEntityType), data.VhId.Value, data.Type.Name);

            this.SpawnType(data.Type, data.VhId, data.TeamId, data.Initializer);
        }

        public void Revert(VhId eventId, SpawnEntityType data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Attempting to revert spawn EntityType {Id}, {Type}", nameof(EntityService), nameof(Revert), nameof(SpawnEntityType), data.VhId.Value, data.Type.Name);

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
            _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Attempting to revert soft despawn Entity {Id}", nameof(EntityService), nameof(Revert), nameof(SpawnEntityType), data.VhId.Value);

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

        private EntityId SpawnDescriptor(VhId descriptorId, in VhId vhid, in TeamId teamId, EntityInitializerDelegate? initializerDelegate)
        {
            EntityInitializer initializer = this.GetDescriptorEngine(descriptorId).Spawn(vhid, teamId, out EntityId id);
            this.Add(id);

            initializerDelegate?.Invoke(this, ref initializer, in id);

            return id;
        }

        private EntityId SpawnType(IEntityType type, in VhId vhid, in TeamId teamId, EntityInitializerDelegate? initializerDelegate)
        {
            EntityInitializer initializer = this.GetDescriptorEngine(type.Descriptor.Id).Spawn(vhid, teamId, out EntityId id);
            this.Add(id);

            _types.GetConfiguration(type).Initialize(this, ref initializer, in id);

            initializerDelegate?.Invoke(this, ref initializer, in id);

            return id;
        }

        private void SoftDespawnEntity(EntityId id)
        {
            DescriptorId descriptorId = this.QueryById<DescriptorId>(id, out GroupIndex groupIndex);
            this.GetDescriptorEngine(descriptorId.Value).SoftDespawn(in id, groupIndex);
        }

        private void RevertSoftDespawnEntity(EntityId id)
        {
            DescriptorId descriptorId = this.QueryById<DescriptorId>(id, out GroupIndex groupIndex);
            this.GetDescriptorEngine(descriptorId.Value).RevertSoftDespawn(in id, groupIndex);
        }

        private void HardDespawnEntity(EntityId id)
        {
            DescriptorId descriptorId = this.QueryById<DescriptorId>(id, out GroupIndex groupIndex);
            this.GetDescriptorEngine(descriptorId.Value).HardDespawn(in id, groupIndex);

            this.Remove(id);
        }
    }
}
