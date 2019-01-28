using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Networking.Enums;
using VoidHuntersRevived.Networking.Implementations;
using VoidHuntersRevived.Networking.Interfaces;
using VoidHuntersRevived.Networking.Peers;

namespace VoidHuntersRevived.Networking.Groups
{
    public class ServerGroup : Group
    {
        private ServerPeer _server;
        private List<NetConnection> _connections;

        public ServerGroup(Int64 id, ServerPeer peer) : base(id, peer)
        {
            _server = peer;
            _connections = new List<NetConnection>();

            this.Users.OnAdd += this.HandleUserAdded;
            this.Users.OnRemove += this.HandleUserRemoved;
        }

        #region Send Methods
        public override void SendMessage(NetOutgoingMessage msg, NetDeliveryMethod method = NetDeliveryMethod.UnreliableSequenced, int sequenceChannel = 0)
        {
            if(_connections.Count > 0)
                _server.SendMessage(msg, _connections, method, sequenceChannel);
        }
        #endregion

        #region MessageType Handlers
        /// <summary>
        /// A client should never be sending the server a user joined command.
        /// This will auto disconnect the user who sent it
        /// </summary>
        /// <param name="im"></param>
        protected override void HandleUserJoined(NetIncomingMessage im)
        {
            im.SenderConnection.Disconnect("Malformed packet.");
        }

        /// <summary>
        /// A client should never be sending the server a user left command.
        /// This will auto disconnect the user who sent it
        /// </summary>
        /// <param name="im"></param>
        protected override void HandleUserLeft(NetIncomingMessage im)
        {
            im.SenderConnection.Disconnect("Malformed packet.");
        }
        #endregion

        #region Event Handlers
        private void HandleUserAdded(object sender, IUser e)
        {
            // When a new user gets added to the group, we want to send a message to all connected users
            // Alerting them of the new user
            var om = this.CreateMessage(MessageType.UserJoined);
            e.Write(om);
            this.SendMessage(om, NetDeliveryMethod.ReliableOrdered);

            // Run the custom updator (if needed)
            var connection = _server.Connections.First(c => c.RemoteUniqueIdentifier == e.Id);
            this.DataHandler?.HandleUserJoined(e, connection);

            // Add the new user to the connections list
            _connections.Add(connection);

            foreach(IUser user in this.Users)
            { // Send every connected user (including the new user) to the new user
                om = this.CreateMessage(MessageType.UserJoined);
                user.Write(om);
                this.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
            }
        }

        private void HandleUserRemoved(object sender, IUser e)
        {
            _connections.Remove(_server.Connections.First(c => c.RemoteUniqueIdentifier == e.Id));
        }
        #endregion
    }
}
