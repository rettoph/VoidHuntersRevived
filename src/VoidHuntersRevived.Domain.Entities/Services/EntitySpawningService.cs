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
using static Guppy.Common.ThrowIf;
using static System.Runtime.InteropServices.JavaScript.JSType;
using VoidHuntersRevived.Common.Messages;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Common.Components;
using Serilog;
using VoidHuntersRevived.Common.Entities.Engines;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class EntitySpawningService : IEntitySpawningService,
        IEventEngine<SpawnEntity>,
        IRevertEventEngine<SpawnEntity>,
        IEventEngine<DespawnEntity>,
        IRevertEventEngine<DespawnEntity>
    {
        private readonly IEntitySerializationService _serialization;
        private readonly ILogger _logger;
        private readonly IEntityFactory _factory;
        private readonly IEntityFunctions _functions;
        private readonly EntityTypeService _types;
        private readonly EntityIdService _entities;
        private readonly HashCache<VhId> _destroyed;
        private readonly Dictionary<VhId, EntityData> _backups;

        public EntitySpawningService(IEntitySerializationService serialization, ILogger logger, EnginesRoot enginesRoot, EntityTypeService types, EntityIdService entities)
        {
            _serialization = serialization;
            _logger = logger;
            _factory = enginesRoot.GenerateEntityFactory();
            _functions = enginesRoot.GenerateEntityFunctions();
            _types = types;
            _entities = entities;
            _destroyed = new HashCache<VhId>(TimeSpan.FromSeconds(5));
            _backups = new Dictionary<VhId, EntityData>();
        }

        private EntityId Spawn(IEntityType type, VhId vhid, EntityInitializerDelegate? initializerDelegate)
        {
            EntityInitializer initializer = type.Descriptor.SpawnEntity(_factory, vhid);

            _types.GetConfiguration(type).Initialize(ref initializer);

            initializerDelegate?.Invoke(ref initializer);

            return _entities.Add(vhid, initializer.EGID, type);
        }

        private void Despawn(VhId vhid)
        {
            EntityId id = _entities.Remove(vhid, out IEntityType type);
            type.Descriptor.DespawnEntity(_functions, in id.EGID);
        }

        public void Process(VhId eventId, SpawnEntity data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Attempting to spawn Entity {Id}, {Type}", nameof(EntitySpawningService), nameof(Process), nameof(SpawnEntity), data.VhId.Value, data.Type.Name);

            if (_destroyed.Remove(data.VhId) != -1)
            {
                _logger.Warning("{ClassName}::{MethodName}<{EventName}> - Unable to spawn Entity {Id}, Destroyed Count: {Count}", nameof(EntitySpawningService), nameof(Process), nameof(SpawnEntity), data.VhId.Value, _destroyed.Count(data.VhId));

                return;
            }

            this.Spawn(data.Type, data.VhId, data.Initializer);
        }

        public void Revert(VhId eventId, SpawnEntity data)
        {
            _logger.Verbose("{ClassName}::{MethodName}<{EventName}> - Attempting to Revert Entity spawn {Id}, {Type}", nameof(EntitySpawningService), nameof(Revert), nameof(SpawnEntity), data.VhId.Value, data.Type.Name);

            if (_destroyed.Add(data.VhId) != 0)
            {
                _logger.Warning("{ClassName}::{MethodName}<{EventName}> - Unable to Revert Entity spawn {Id}, Destroyed Count: {Count}", nameof(EntitySpawningService), nameof(Revert), nameof(SpawnEntity), data.VhId.Value, _destroyed.Count(data.VhId));

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
    }
}
