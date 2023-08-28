using Guppy.Common.Collections;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Events;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Messages;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Common.Entities.Engines;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal partial class EntityService :
        IEventEngine<SpawnEntityDescriptor>,
        IRevertEventEngine<SpawnEntityDescriptor>,
        IEventEngine<SpawnEntityType>,
        IRevertEventEngine<SpawnEntityType>,
        IEventEngine<DespawnEntity>,
        IRevertEventEngine<DespawnEntity>
    {
        public EntityId Spawn(IEntityType type, VhId vhid, EntityInitializerDelegate? initializer)
        {
            _events.Publish(NameSpace<EntityService>.Instance, new SpawnEntityType()
            {
                Type = type,
                VhId = vhid,
                Initializer = initializer
            });

            return this.GetId(vhid);
        }

        public void Despawn(VhId vhid)
        {
            _events.Publish(NameSpace<EntityService>.Instance, new DespawnEntity()
            {
                VhId = vhid
            });
        }

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

        public void Process(VhId eventId, SpawnEntityDescriptor data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Attempting to spawn EntityDescriptor {Id}, {Descriptor}", nameof(EntityService), nameof(Process), nameof(SpawnEntityDescriptor), data.VhId.Value, data.Descriptor.Name);

            this.SpawnDescriptor(data.Descriptor, data.VhId, data.Initializer);
        }

        public void Revert(VhId eventId, SpawnEntityDescriptor data)
        {
            throw new NotImplementedException();
        }

        public void Process(VhId eventId, SpawnEntityType data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Attempting to spawn EntityType {Id}, {Type}", nameof(EntityService), nameof(Process), nameof(SpawnEntityType), data.VhId.Value, data.Type.Name);

            this.SpawnType(data.Type, data.VhId, data.Initializer);
        }

        public void Revert(VhId eventId, SpawnEntityType data)
        {
            throw new NotImplementedException();
        }

        public void Process(VhId eventId, DespawnEntity data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Attempting to despawn Entity {Id}", nameof(Services.EntityService), nameof(Process), nameof(Common.Entities.Events.DespawnEntity), data.VhId.Value);

            this.DespawnEntity(data.VhId);
        }

        public void Revert(VhId eventId, DespawnEntity data)
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

        private void DespawnEntity(VhId vhid)
        {
            EntityId id = this.Remove(vhid);
            _descriptors.Value.Despawn(_functions, in id);
        }
    }
}
