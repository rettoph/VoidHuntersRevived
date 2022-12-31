using Guppy.Common.Collections;
using Guppy.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Mappers
{
    public abstract class NetIdMap
    {
        private INetId _current;
        private readonly Queue<INetId> _availableIds;
        private readonly Map<INetId, int> _entityIds;

        public NetIdMap(INetId zero)
        {
            _availableIds = new Queue<INetId>();
            _entityIds = new Map<INetId, int>();
            _current = zero;
        }

        public INetId Add(int entityId)
        {
            var id = this.GetNextNetId();

            _entityIds.Add(id, entityId);

            return id;
        }

        public void Remove(INetId id)
        {
            _entityIds.Remove(id);

            _availableIds.Enqueue(id);
        }

        public void Remove(int entityId)
        {
            var id = this.GetId(entityId);

            this.Remove(id);
        }

        public INetId GetId(int entityId)
        {
            return _entityIds[entityId];
        }

        public int GetId(INetId id)
        {
            return _entityIds[id];
        }

        private INetId GetNextNetId()
        {
            if(_availableIds.TryDequeue(out var id))
            {
                return _availableIds.Dequeue();
            }

            _current = _current.Next();

            return _current;
        }
    }
}
