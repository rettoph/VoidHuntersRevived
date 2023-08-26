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
        private readonly HashCache<VhId> _destroyed = new HashCache<VhId>(TimeSpan.FromSeconds(5));
        private readonly Dictionary<VhId, EntityData> _backups = new Dictionary<VhId, EntityData>();

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

            if (_destroyed.Remove(data.VhId) != -1)
            {
                _logger.Warning("{ClassName}::{MethodName}<{EventName}> - Unable to spawn EntityDescriptor {Id}, Destroyed Count: {Count}", nameof(EntityService), nameof(Process), nameof(SpawnEntityDescriptor), data.VhId.Value, _destroyed.Count(data.VhId));

                return;
            }

            this.SpawnDescriptor(data.Descriptor, data.VhId, data.Initializer);
        }

        public void Revert(VhId eventId, SpawnEntityDescriptor data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Attempting to Revert EntityDescriptor spawn {Id}, {Descriptor}", nameof(EntityService), nameof(Revert), nameof(SpawnEntityDescriptor), data.VhId.Value, data.Descriptor.Name);

            if (_destroyed.Add(data.VhId) != 0)
            {
                _logger.Warning("{ClassName}::{MethodName}<{EventName}> - Unable to Revert EntityDescriptor spawn {Id}, Destroyed Count: {Count}", nameof(EntityService), nameof(Revert), nameof(SpawnEntityDescriptor), data.VhId.Value, _destroyed.Count(data.VhId));

                return;
            }
            this.DespawnEntity(data.VhId);
        }

        public void Process(VhId eventId, SpawnEntityType data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Attempting to spawn EntityType {Id}, {Type}", nameof(EntityService), nameof(Process), nameof(SpawnEntityType), data.VhId.Value, data.Type.Name);

            if (_destroyed.Remove(data.VhId) != -1)
            {
                _logger.Warning("{ClassName}::{MethodName}<{EventName}> - Unable to spawn EntityType {Id}, Destroyed Count: {Count}", nameof(EntityService), nameof(Process), nameof(SpawnEntityType), data.VhId.Value, _destroyed.Count(data.VhId));
                return;
            }

            this.SpawnType(data.Type, data.VhId, data.Initializer);
        }

        public void Revert(VhId eventId, SpawnEntityType data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Attempting to Revert EntityType spawn {Id}, {Type}", nameof(EntityService), nameof(Revert), nameof(SpawnEntityType), data.VhId.Value, data.Type.Name);

            if (_destroyed.Add(data.VhId) != 0)
            {
                _logger.Warning("{ClassName}::{MethodName}<{EventName}> - Unable to Revert EntityType spawn {Id}, Destroyed Count: {Count}", nameof(EntityService), nameof(Revert), nameof(SpawnEntityType), data.VhId.Value, _destroyed.Count(data.VhId));
                return;
            }
            this.DespawnEntity(data.VhId);
        }

        public void Process(VhId eventId, DespawnEntity data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Attempting to despawn Entity {Id}", nameof(Services.EntityService), nameof(Process), nameof(Common.Entities.Events.DespawnEntity), data.VhId.Value);

            if (_destroyed.Add(data.VhId) != 0)
            {
                _logger.Warning("{ClassName}::{MethodName}<{EventName}> - Unableto despawn Entity {Id}, Destroyed Count: {Count}", nameof(Services.EntityService), nameof(Process), nameof(Common.Entities.Events.DespawnEntity), data.VhId.Value, _destroyed.Count(data.VhId));

                return;
            }

            EntityData backup = this.Serialize(this.GetId(data.VhId));
            _backups[data.VhId] = backup;
            this.DespawnEntity(data.VhId);
        }

        public void Revert(VhId eventId, DespawnEntity data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Attempting to Revert Entity despawn {Id}", nameof(Services.EntityService), nameof(Revert), nameof(Common.Entities.Events.DespawnEntity), data.VhId.Value);

            if (_destroyed.Count(data.VhId) != 0)
            {
                _logger.Warning("{ClassName}::{MethodName}<{EventName}> - Unable to Revert Entity despawn {Id}, Destroyed Count: {Count}", nameof(Services.EntityService), nameof(Revert), nameof(Common.Entities.Events.DespawnEntity), data.VhId.Value, _destroyed.Count(data.VhId));
                return;
            }

            if (!_backups.Remove(data.VhId, out EntityData? backup))
            {
                throw new Exception();
            }

            this.Deserialize(VhId.Empty, backup, null, true);
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
