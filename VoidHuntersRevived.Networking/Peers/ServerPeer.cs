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

        public ServerPeer(String appIdentifier, Int32 port, INetworkGame game, ILogger logger)
            : base(appIdentifier, game, logger)
        {
            // Update configuration
            _configuration.Port = port;

            _server = new NetServer(_configuration);
            _peer = _server;
        }

        public override void SendMessage(NetOutgoingMessage om, NetDeliveryMethod method = NetDeliveryMethod.UnreliableSequenced)
        {
            throw new NotImplementedException();
        }
    }
}
