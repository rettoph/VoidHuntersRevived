using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Networking.Implementations
{
    public abstract class NetworkEntity : Entity, INetworkEntity
    {
        private Boolean _dirty;

        public Boolean Dirty
        {
            get
            {
                return _dirty;
            }
            set
            {
                if(value != _dirty)
                {
                    _dirty = value;

                    if (!_dirty)
                        this.OnDirty?.Invoke(this, this);
                }
            }
        }
        public Int64 Id { get; private set; }

        public event EventHandler<INetworkEntity> OnDirty;

        public NetworkEntity(Int64 id, EntityInfo info, IGame game) : base(info, game)
        {
            this.Id = id;
        }
        public NetworkEntity(EntityInfo info, IGame game) : base(info, game)
        {
            this.Id = BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0);
        }



        public abstract void Read(NetIncomingMessage im);
        public abstract void Write(NetOutgoingMessage om);
    }
}
