using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Collections;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Networking.Collections
{
    public class NetworkEntityCollection : GameCollection<INetworkEntity>
    {
        private Dictionary<Int64, INetworkEntity> _networkEntityTable;

        public NetworkEntityCollection()
        {
            _networkEntityTable = new Dictionary<Int64, INetworkEntity>();
        }

        protected override bool add(INetworkEntity item)
        {
            if (base.add(item))
            {
                _networkEntityTable.Add(item.Id, item);

                return true;
            }

            return false;
        }

        protected override bool remove(INetworkEntity item)
        {
            if (base.remove(item))
            {
                _networkEntityTable.Remove(item.Id);

                return true;
            }

            return false;
        }

        public INetworkEntity GetById(Int64 id)
        {
            if (_networkEntityTable.ContainsKey(id))
                return _networkEntityTable[id];

            return default(INetworkEntity);
        }

        public INetworkEntity RemoveById(Int64 id)
        {
            return this.Remove(this.GetById(id));
        }
    }
}
