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
        public EntityId Spawn(IEntityType type, VhId vhid, EntityInitializerDelegate? initializer)
        {
            this.Simulation.Publish(NameSpace<EntityService>.Instance, new SpawnEntityType()
            {
                Type = type,
                VhId = vhid,
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

        private bool _flushing;
        public void Flush()
        {
            _scheduler.SubmitEntities();
        }

        public void Despawn(EGID egid)
        {
            this.Despawn(this.GetId(egid).VhId);
        }

        public void Despawn(EntityId id)
        {
            this.Despawn(id.VhId);
        }

        public bool Exists(EntityId id)
        {
            if(this.TryQueryById<EntityStatus>(id, out EntityStatus status))
            {
                return status.Status == EntityStatusType.Spawned;
            }

            return false;
        }

        public bool Exists(in GroupIndex groupIndex)
        {
            if (this.TryQueryByGroupIndex<EntityStatus>(in groupIndex, out EntityStatus status))
            {
                return status.Status == EntityStatusType.Spawned;
            }

            return false;
        }

        public void Process(VhId eventId, SpawnEntityDescriptor data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Attempting to spawn EntityDescriptor {Id}, {Descriptor}", nameof(EntityService), nameof(Process), nameof(SpawnEntityDescriptor), data.VhId.Value, data.Descriptor.Name);

            this.SpawnDescriptor(data.Descriptor, data.VhId, data.Initializer);
        }

        public void Revert(VhId eventId, SpawnEntityDescriptor data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Attempting to revert spawn EntityDescriptor {Id}, {Descriptor}", nameof(EntityService), nameof(Revert), nameof(SpawnEntityDescriptor), data.VhId.Value, data.Descriptor.Name);

            EntityId id = this.GetId(data.VhId);
            this.SoftDespawnEntity(id);
            this.HardDespawnEntity(id);
        }

        public void Process(VhId eventId, SpawnEntityType data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Attempting to spawn EntityType {Id}, {Type}", nameof(EntityService), nameof(Process), nameof(SpawnEntityType), data.VhId.Value, data.Type.Name);

            this.SpawnType(data.Type, data.VhId, data.Initializer);
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
                ref EntityStatus status = ref this.QueryById<EntityStatus>(id, out GroupIndex groupIndex);
                status.Status = EntityStatusType.Spawned;
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

        private EntityId SpawnDescriptor(VoidHuntersEntityDescriptor descriptor, VhId vhid, EntityInitializerDelegate? initializerDelegate)
        {
            EntityInitializer initializer = _descriptors.Value.Spawn(descriptor, _factory, vhid, out EntityId id);
            this.Add(id);

            initializerDelegate?.Invoke(this, ref initializer, in id);

            return id;
        }

        private EntityId SpawnType(IEntityType type, VhId vhid, EntityInitializerDelegate? initializerDelegate)
        {
            EntityInitializer initializer = _descriptors.Value.Spawn(type.Descriptor, _factory, vhid, out EntityId id);
            this.Add(id);

            _types.GetConfiguration(type).Initialize(this, ref initializer, in id);

            initializerDelegate?.Invoke(this, ref initializer, in id);

            return id;
        }

        private void SoftDespawnEntity(EntityId id)
        {
            _descriptors.Value.SoftDespawn(this, in id);
        }

        private void HardDespawnEntity(EntityId id)
        {
            this.Remove(id);

            _descriptors.Value.HardDespawn(this, _functions, in id);
        }
    }
}
