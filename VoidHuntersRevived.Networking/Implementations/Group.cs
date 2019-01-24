using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Networking.Implementations
{
    public abstract class Group : IGroup
    {
        public Guid Id { get; protected set; }
        protected NetPeer _peer;
        private NetOutgoingMessage _om;

        public Group(NetPeer peer)
        {
            _peer = peer;
        }

        public NetOutgoingMessage CreateMessage()
        {
            _om = _peer.CreateMessage();
            _om.Write(this.Id.ToByteArray());

            return _om;
        }

        public abstract void SendMessage(NetOutgoingMessage om, NetDeliveryMethod method = NetDeliveryMethod.UnreliableSequenced);

        public void SendMessage(NetOutgoingMessage om, IUser user, NetDeliveryMethod method = NetDeliveryMethod.UnreliableSequenced)
        {
            throw new NotImplementedException();
        }
    }
}
