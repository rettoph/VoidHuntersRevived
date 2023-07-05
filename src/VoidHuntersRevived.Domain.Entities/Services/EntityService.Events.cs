﻿using System;
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
using VoidHuntersRevived.Common.Entities.Enums;

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

        private IdMap Create(EntityType type, VhId vhid, EntityInitializerDelegate? initializerDelegate)
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
            if (!this.TryGetIdMap(vhid, out IdMap id))
            {
                return;
            }

            _types[vhid].DestroyEntity(_functions, in id.EGID);

            _ids.Remove(id.VhId, id.EGID);
            _types.Remove(id.VhId);
        }

        public void Process(VhId eventId, CreateEntity data)
        {
            this.Create(data.Type, data.VhId, data.Initializer);
        }

        public void Revert(VhId eventId, CreateEntity data)
        {
            _destroyed.Add(eventId);
            this.Destroy(data.VhId);
        }

        public void Process(VhId eventId, DestroyEntity data)
        {
            EntityData backup = _engines.Serialization.Serialize(this.GetIdMap(data.VhId));

            _destroyed.Add(eventId);
            _backups.Add(eventId, backup);
            this.Destroy(data.VhId);
        }

        public void Revert(VhId eventId, DestroyEntity data)
        {
            throw new NotImplementedException();
        }
    }
}
