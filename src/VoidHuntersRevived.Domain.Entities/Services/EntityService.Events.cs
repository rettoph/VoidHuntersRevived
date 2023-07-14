using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Events;
using VoidHuntersRevived.Common.Entities.Engines;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Common.Components;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal partial class EntityService : 
        IEventEngine<CreateEntity>,
        IRevertEventEngine<CreateEntity>, 
        IEventEngine<DestroyEntity>,
        IRevertEventEngine<DestroyEntity>
    {
        private HashCache<VhId> _destroyed = new HashCache<VhId>(TimeSpan.FromSeconds(5));
        private Dictionary<VhId, EntityData> _backups = new Dictionary<VhId, EntityData>();

        private IdMap Create(IEntityType type, VhId vhid, EntityInitializerDelegate? initializerDelegate)
        {
            EntityInitializer initializer = type.CreateEntity(_factory);

            initializer.Init(new EntityVhId() { Value = vhid });
            _entityTypes.GetConfiguration(type).Initialize(ref initializer);

            initializerDelegate?.Invoke(ref initializer);

            IdMap idMap = new IdMap(initializer.EGID, vhid);
            _ids.TryAdd(vhid, initializer.EGID, idMap);
            _types.Add(vhid, type);

            return idMap;
        }

        private void Destroy(VhId vhid)
        {
            ref IdMap id = ref _ids.TryGetRef(vhid, out bool isNullRef);
            if (isNullRef)
            {
                throw new NullReferenceException();
            }

            id.Destroyed = true;
            _types[vhid].DestroyEntity(_functions, in id.EGID);
            _removed.Enqueue(id);
        }

        public void Process(VhId eventId, CreateEntity data)
        {
            _logger.Debug("Attempting to Create Entity {Id}, {Type}", data.VhId.Value, data.Type.Name);

            if (_destroyed.Remove(data.VhId) != -1)
            {
                _logger.Warning("Unable to Create Entity {Id}, Destroyed Count: {Count}", data.VhId.Value, _destroyed.Count(data.VhId));

                return;
            }

            this.Create(data.Type, data.VhId, data.Initializer);
        }

        public void Revert(VhId eventId, CreateEntity data)
        {
            _logger.Debug("Attempting to Revert Entity Creation {Id}, {Type}", data.VhId.Value, data.Type.Name);

            if (_destroyed.Add(data.VhId) != 0)
            {
                _logger.Warning("Unable to Revert Entity Creation {Id}, Destroyed Count: {Count}", data.VhId.Value, _destroyed.Count(data.VhId));

                return;
            }

            this.Destroy(data.VhId);
        }

        public void Process(VhId eventId, DestroyEntity data)
        {
            _logger.Debug("Attempting to Destroy Entity {Id}", data.VhId.Value);

            if (_destroyed.Add(data.VhId) != 0)
            {
                _logger.Warning("Unableto Destroy Entity {Id}, Destroyed Count: {Count}", data.VhId.Value, _destroyed.Count(data.VhId));

                return;
            }

            EntityData backup = _serialization.Serialize(this.GetIdMap(data.VhId));

            _backups[data.VhId] = backup;
            this.Destroy(data.VhId);
        }

        public void Revert(VhId eventId, DestroyEntity data)
        {
            _logger.Debug("Attempting to Revert Entity Destruction {Id}", data.VhId.Value);

            if (_destroyed.Count(data.VhId) != 0)
            {
                _logger.Warning("Unable to Revert Entity Destruction {Id}, Destroyed Count: {Count}", data.VhId.Value, _destroyed.Count(data.VhId));

                return;
            }

            if(!_backups.Remove(data.VhId, out EntityData? backup))
            {
                throw new Exception();
            }

            _serialization.Deserialize(VhId.Empty, backup, true);
        }
    }
}
