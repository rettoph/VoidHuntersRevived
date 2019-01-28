using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using VoidHuntersRevived.Networking.Collections;
using VoidHuntersRevived.Networking.Enums;
using VoidHuntersRevived.Networking.Groups;
using VoidHuntersRevived.Networking.Implementations;
using VoidHuntersRevived.Networking.Interfaces;
using VoidHuntersRevived.Networking.Responses;

namespace VoidHuntersRevived.Networking.Peers
{
    public class ServerPeer : Peer
    {
        protected NetServer _server;
        private NetIncomingMessage _im;

        public List<NetConnection> Connections { get { return _server.Connections; } }

        /// <summary>
        /// The IAuthenticator instance that is used to
        /// authenticate incomming connections
        /// </summary>
        public IAuthenticator Authenticator { get; set; }

        // A secret list of approved (but not connected) users
        private UserCollection _approvedUsers;

        public ServerPeer(String appIdentifier, Int32 port, INetworkGame game, ILogger logger)
            : base(appIdentifier, game, logger)
        {
            // Update configuration
            _configuration.Port = port;

            // Create the underlying peer object
            _server = new NetServer(_configuration);
            _peer = _server;

            // Create new default handlers
            this.Authenticator = new Authenticator();

            // Create the containing lists
            _approvedUsers = new UserCollection();

            // Create the new group collection
            this.Groups = new GroupCollection(this, typeof(ServerGroup));
        }

        /// <summary>
        /// Read any new incoming messages and handle them accordingly
        /// As this is a server peer, certain message types that cannot
        /// exists on a client are handled here
        /// </summary>
        public override void Update()
        {
            while ((_im = _peer.ReadMessage()) != null)
            { // Read any new incoming messages
                Logger.LogInformation(_im.MessageType.ToString());

                switch (_im.MessageType)
                {
                    case NetIncomingMessageType.ConnectionApproval:
                        this.HandleConnectionApprovalMessage(_im);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        this.HandleStatusChanged(_im);
                        break;
                }
            }

            // Update the underlying group
            base.Update();
        }

        #region MessageType Handlers
        /// <summary>
        /// Parse an incoming connection approval message into the
        /// clients claimed user, then run the current authenticator
        /// </summary>
        /// <param name="im"></param>
        protected virtual void HandleConnectionApprovalMessage(NetIncomingMessage im)
        {
            Logger.LogDebug("New incoming connection...");

            var target = (MessageTarget)im.SenderConnection.RemoteHailMessage.ReadByte();
            var messageType = im.SenderConnection.RemoteHailMessage.ReadString();
            AuthenticationResponse authResponse = this.Authenticator.Authenticate(this, im.SenderConnection);

            // Ensure that the incoming message contains the peer target type

            if (target != MessageTarget.Peer || messageType != "network:user:connection-request")
            {
                Logger.LogWarning("Connection denied. Malformed packet.");
                im.SenderConnection.Deny("Malformed packet");

            }
            else if(!authResponse.Authenticated)
            {
                Logger.LogWarning("Connection denied. Unauthorized.");
                im.SenderConnection.Deny("Unauthorized");
            }
            else
            {
                Logger.LogDebug("Connection approved.");

                // Add the user to the hidden approved users list
                _approvedUsers.Add(authResponse.User);

                // Approve of their connection
                im.SenderConnection.Approve();
            }
        }

        private void HandleStatusChanged(NetIncomingMessage im)
        {
            Logger.LogTrace(im.SenderConnection.Status.ToString());

            IUser user;

            switch (_im.SenderConnection.Status)
            {
                case NetConnectionStatus.None:
                    break;
                case NetConnectionStatus.InitiatedConnect:
                    break;
                case NetConnectionStatus.ReceivedInitiation:
                    break;
                case NetConnectionStatus.RespondedAwaitingApproval:
                    break;
                case NetConnectionStatus.RespondedConnect:
                    break;
                case NetConnectionStatus.Connected:
                    // When the connection status changes to connected we can remove the connections user
                    // from the _approvedUsers collection and add it to the global users collection
                    user = _approvedUsers.GetById(im.SenderConnection.RemoteUniqueIdentifier);
                    this.Users.Add(user);
                    _approvedUsers.Remove(user);
                    break;
                case NetConnectionStatus.Disconnecting:
                    break;
                case NetConnectionStatus.Disconnected:
                    user = this.Users.GetById(im.SenderConnection.RemoteUniqueIdentifier);
                    user.Disconnect();
                    break;
            }

        }

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

        #region Send Methods
        public void SendMessage(NetOutgoingMessage msg, List<NetConnection> recipients, NetDeliveryMethod method = NetDeliveryMethod.UnreliableSequenced, Int32 sequenceChannel = 0)
        {
            _server.SendMessage(msg, recipients, method, sequenceChannel);
        }
        public void SendMessage(NetOutgoingMessage msg, NetConnection recipient, NetDeliveryMethod method = NetDeliveryMethod.UnreliableSequenced, Int32 sequenceChannel = 0)
        {
            _server.SendMessage(msg, recipient, method, sequenceChannel);
        }
        public override void SendMessage(NetOutgoingMessage msg, NetDeliveryMethod method = NetDeliveryMethod.UnreliableSequenced, int sequenceChannel = 0)
        {
            _server.SendMessage(msg, _server.Connections, method, sequenceChannel);
        }
        #endregion
    }
}
