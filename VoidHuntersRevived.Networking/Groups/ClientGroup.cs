using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;
using VoidHuntersRevived.Networking.Implementations;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Networking.Groups
{
    public class ClientGroup : Group
    {
        public ClientGroup(long id, IPeer peer) : base(id, peer)
        {
        }

        public override NetOutgoingMessage CreateMessage()
        {
            throw new NotImplementedException();
        }

        public override void SendMessage(NetOutgoingMessage msg, NetDeliveryMethod method = NetDeliveryMethod.UnreliableSequenced, int sequenceChannel = 0)
        {
            this.Peer.SendMessage(msg, method, sequenceChannel);
        }

        #region MessageType Handlers
        protected override void HandleUserJoined(NetIncomingMessage im)
        {
            throw new NotImplementedException();
        }

        protected override void HandleUserLeft(NetIncomingMessage im)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
