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
        public Queue<INetworkEntity> DirtyNetworkEntityQueue { get; private set; }

        public NetworkEntityCollection()
        {
            _networkEntityTable = new Dictionary<Int64, INetworkEntity>();
            this.DirtyNetworkEntityQueue = new Queue<INetworkEntity>();
        }

        protected override bool add(INetworkEntity item)
        {
            if (base.add(item))
            {
                _networkEntityTable.Add(item.Id, item);
                item.OnDirty += this.HandleNetworkEntityDirty;

                return true;
            }

            return false;
        }

        protected override bool remove(INetworkEntity item)
        {
            if (base.remove(item))
            {
                _networkEntityTable.Remove(item.Id);
                item.OnDirty -= this.HandleNetworkEntityDirty;

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

        #region EventHandlers
        private void HandleNetworkEntityDirty(object sender, INetworkEntity e)
        {
            // Add the dirty entity to the dirty entities queue
            this.DirtyNetworkEntityQueue.Enqueue(e);
        }
        #endregion
    }
}
