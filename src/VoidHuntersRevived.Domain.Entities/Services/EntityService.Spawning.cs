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

            while (_modifications.TryDequeue(out (EntityModification type, EntityId id) modification))
            {
                switch(modification.type)
                {
                    case EntityModification.SoftSpawn:
                        this.SoftSpawnEntity(modification.id);
                        break;
                    case EntityModification.SoftDespawn:
                        this.SoftDespawnEntity(modification.id);
                        break;
                    case EntityModification.HardDespawn:
                        this.HardDespawnEntity(modification.id);
                        break;
                    case EntityModification.RevertSoftDespawn:
                        this.RevertSoftDespawnEntity(modification.id);
                        break;
                }
            }

            if(_modifications.Count > 0)
            {

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
            this.SpawnEntity(data.Type, data.VhId, data.TeamId, data.Initializer);
        }

        public void Revert(VhId eventId, SpawnEntity data)
        {
            EntityId id = this.GetId(data.VhId);
            this.EnqueuEntityModification(EntityModification.SoftDespawn, id);
            this.EnqueuEntityModification(EntityModification.HardDespawn, id);
        }

        public void Process(VhId eventId, SoftDespawnEntity data)
        {
            EntityId id = this.GetId(data.VhId);
            this.EnqueuEntityModification(EntityModification.SoftDespawn, id);
        }

        public void Revert(VhId eventId, SoftDespawnEntity data)
        {
            if(this.TryGetId(data.VhId, out EntityId id))
            {
                this.EnqueuEntityModification(EntityModification.RevertSoftDespawn, id);
            }
            else
            {
                _logger.Warning("{ClassName}::{MethodName}<{EventName}> - Unable to revert soft despawn Entity {Id}", nameof(EntityService), nameof(Revert), nameof(Events.SoftDespawnEntity), data.VhId.Value);
            }
        }

        public void Process(VhId eventId, HardDespawnEntity data)
        {
            EntityId id = this.GetId(data.VhId);
            this.EnqueuEntityModification(EntityModification.HardDespawn, id);
        }

        public void Revert(VhId eventId, HardDespawnEntity data)
        {
            throw new NotImplementedException();
        }

        private EntityId SpawnEntity(IEntityType type, in VhId vhid, in Id<ITeam> teamId, EntityInitializerDelegate? initializerDelegate)
        {
            EntityInitializer initializer = this.GetDescriptorEngine(type.Descriptor.Id).HardSpawn(vhid, teamId, out EntityId id);
            this.AddId(id);
            this.EnqueuEntityModification(EntityModification.SoftSpawn, id);

            _types.GetConfiguration(type).Initialize(this, ref initializer, in id);

            initializerDelegate?.Invoke(this, ref initializer, in id);

            return id;
        }

        private void SoftSpawnEntity(EntityId id)
        {
            Id<VoidHuntersEntityDescriptor> descriptorId = this.QueryById<Id<VoidHuntersEntityDescriptor>>(id, out GroupIndex groupIndex);
            this.GetDescriptorEngine(descriptorId).SoftSpawn(in id, groupIndex);
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

            this.RemoveId(id);
        }
    }
}
