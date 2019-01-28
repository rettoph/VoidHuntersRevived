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
            this.MessageTypeHandlers.Add("network:user:joined", this.HandleUserJoined);
            this.MessageTypeHandlers.Add("network:user:left", this.HandleUserLeft);
        }

        public override void SendMessage(NetOutgoingMessage msg, NetDeliveryMethod method = NetDeliveryMethod.UnreliableSequenced, int sequenceChannel = 0)
        {
            this.Peer.SendMessage(msg, method, sequenceChannel);
        }

        #region MessageType Handlers
        protected override void HandleUserJoined(NetIncomingMessage im)
        {
            var user = new User(im.ReadInt64(), im.ReadString());
            this.Users.Add(user);
        }

        protected override void HandleUserLeft(NetIncomingMessage im)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
