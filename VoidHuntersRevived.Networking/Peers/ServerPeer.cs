using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using VoidHuntersRevived.Networking.Implementations;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Networking.Peers
{
    public class ServerPeer : Peer
    {
        protected NetServer _server;
        private NetIncomingMessage _im;

        public ServerPeer(String appIdentifier, Int32 port, INetworkGame game, ILogger logger)
            : base(appIdentifier, game, logger)
        {
            // Update configuration
            _configuration.Port = port;

            _server = new NetServer(_configuration);
            _peer = _server;
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
                _logger.LogInformation(_im.MessageType.ToString());

                switch (_im.MessageType)
                {
                    case NetIncomingMessageType.ConnectionApproval:
                        this.HandleConnectionApprovalMessage(_im);
                        break;
                }
            }
        }

        #region MessageTypeHandlers
        /// <summary>
        /// Parse an incoming connection approval message into the
        /// clients claimed user, then run the current authenticator
        /// </summary>
        /// <param name="im"></param>
        protected virtual void HandleConnectionApprovalMessage(NetIncomingMessage im)
        {

        }
        #endregion
    }
}
