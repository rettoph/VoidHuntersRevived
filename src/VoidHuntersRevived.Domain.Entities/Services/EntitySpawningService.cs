using Guppy.Common.Collections;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Events;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Utilities;
using Serilog;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Descriptors;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class EntitySpawningService : IEntitySpawningService,
        IEventEngine<SpawnEntityDescriptor>,
        IRevertEventEngine<SpawnEntityDescriptor>,
        IEventEngine<SpawnEntityType>,
        IRevertEventEngine<SpawnEntityType>,
        IEventEngine<DespawnEntity>,
        IRevertEventEngine<DespawnEntity>
    {
        private readonly IEntitySerializationService _serialization;
        private readonly IEntityDescriptorService _descriptors;
        private readonly ILogger _logger;
        private readonly IEntityFactory _factory;
        private readonly IEntityFunctions _functions;
        private readonly EntityTypeService _types;
        private readonly EntityIdService _entities;
        private readonly HashCache<VhId> _destroyed;
        private readonly Dictionary<VhId, EntityData> _backups;

        public EntitySpawningService(IEntitySerializationService serialization, IEntityDescriptorService descriptors, ILogger logger, EnginesRoot enginesRoot, EntityTypeService types, EntityIdService entities)
        {
            _serialization = serialization;
            _descriptors = descriptors;
            _logger = logger;
            _factory = enginesRoot.GenerateEntityFactory();
            _functions = enginesRoot.GenerateEntityFunctions();
            _types = types;
            _entities = entities;
            _destroyed = new HashCache<VhId>(TimeSpan.FromSeconds(5));
            _backups = new Dictionary<VhId, EntityData>();
        }

        public void Process(VhId eventId, SpawnEntityDescriptor data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Attempting to spawn EntityDescriptor {Id}, {Descriptor}", nameof(EntitySpawningService), nameof(Process), nameof(SpawnEntityDescriptor), data.VhId.Value, data.Descriptor.Name);

            if (_destroyed.Remove(data.VhId) != -1)
            {
                _logger.Warning("{ClassName}::{MethodName}<{EventName}> - Unable to spawn EntityDescriptor {Id}, Destroyed Count: {Count}", nameof(EntitySpawningService), nameof(Process), nameof(SpawnEntityDescriptor), data.VhId.Value, _destroyed.Count(data.VhId));

                return;
            }

            this.SpawnDescriptor(data.Descriptor, data.VhId, data.Initializer);
        }

        public void Revert(VhId eventId, SpawnEntityDescriptor data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Attempting to Revert EntityDescriptor spawn {Id}, {Descriptor}", nameof(EntitySpawningService), nameof(Revert), nameof(SpawnEntityDescriptor), data.VhId.Value, data.Descriptor.Name);

            if (_destroyed.Add(data.VhId) != 0)
            {
                _logger.Warning("{ClassName}::{MethodName}<{EventName}> - Unable to Revert EntityDescriptor spawn {Id}, Destroyed Count: {Count}", nameof(EntitySpawningService), nameof(Revert), nameof(SpawnEntityDescriptor), data.VhId.Value, _destroyed.Count(data.VhId));

                return;
            }

            this.Despawn(data.VhId);
        }

        public void Process(VhId eventId, SpawnEntityType data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Attempting to spawn EntityType {Id}, {Type}", nameof(EntitySpawningService), nameof(Process), nameof(SpawnEntityType), data.VhId.Value, data.Type.Name);

            if (_destroyed.Remove(data.VhId) != -1)
            {
                _logger.Warning("{ClassName}::{MethodName}<{EventName}> - Unable to spawn EntityType {Id}, Destroyed Count: {Count}", nameof(EntitySpawningService), nameof(Process), nameof(SpawnEntityType), data.VhId.Value, _destroyed.Count(data.VhId));

                return;
            }

            this.SpawnType(data.Type, data.VhId, data.Initializer);
        }

        public void Revert(VhId eventId, SpawnEntityType data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Attempting to Revert EntityType spawn {Id}, {Type}", nameof(EntitySpawningService), nameof(Revert), nameof(SpawnEntityType), data.VhId.Value, data.Type.Name);

            if (_destroyed.Add(data.VhId) != 0)
            {
                _logger.Warning("{ClassName}::{MethodName}<{EventName}> - Unable to Revert EntityType spawn {Id}, Destroyed Count: {Count}", nameof(EntitySpawningService), nameof(Revert), nameof(SpawnEntityType), data.VhId.Value, _destroyed.Count(data.VhId));

                return;
            }

            this.Despawn(data.VhId);
        }

        public void Process(VhId eventId, DespawnEntity data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Attempting to despawn Entity {Id}", nameof(EntitySpawningService), nameof(Process), nameof(DespawnEntity), data.VhId.Value);

            if (_destroyed.Add(data.VhId) != 0)
            {
                _logger.Warning("{ClassName}::{MethodName}<{EventName}> - Unableto despawn Entity {Id}, Destroyed Count: {Count}", nameof(EntitySpawningService), nameof(Process), nameof(DespawnEntity), data.VhId.Value, _destroyed.Count(data.VhId));

                return;
            }

            EntityData backup = _serialization.Serialize(_entities.GetId(data.VhId));

            _backups[data.VhId] = backup;
            this.Despawn(data.VhId);
        }

        public void Revert(VhId eventId, DespawnEntity data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Attempting to Revert Entity despawn {Id}", nameof(EntitySpawningService), nameof(Revert), nameof(DespawnEntity), data.VhId.Value);

            if (_destroyed.Count(data.VhId) != 0)
            {
                _logger.Warning("{ClassName}::{MethodName}<{EventName}> - Unable to Revert Entity despawn {Id}, Destroyed Count: {Count}", nameof(EntitySpawningService), nameof(Revert), nameof(DespawnEntity), data.VhId.Value, _destroyed.Count(data.VhId));

                return;
            }

            if (!_backups.Remove(data.VhId, out EntityData? backup))
            {
                throw new Exception();
            }

            _serialization.Deserialize(VhId.Empty, backup, true);
        }

        private EntityId SpawnDescriptor(VoidHuntersEntityDescriptor descriptor, VhId vhid, EntityInitializerDelegate? initializerDelegate)
        {
            EntityInitializer initializer = _descriptors.Spawn(descriptor, _factory, vhid);

            initializerDelegate?.Invoke(ref initializer);

            return _entities.Add(vhid, initializer.EGID);
        }

        private EntityId SpawnType(IEntityType type, VhId vhid, EntityInitializerDelegate? initializerDelegate)
        {
            EntityInitializer initializer = _descriptors.Spawn(type.Descriptor, _factory, vhid);

            _types.GetConfiguration(type).Initialize(ref initializer);

            initializerDelegate?.Invoke(ref initializer);

            return _entities.Add(vhid, initializer.EGID);
        }

        private void Despawn(VhId vhid)
        {
            EntityId id = _entities.Remove(vhid);
            _descriptors.Despawn(_functions, in id);
        }
    }
}
