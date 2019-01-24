using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using VoidHuntersRevived.Networking.Implementations;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Networking.Peers
{
    /// <summary>
    /// Implementation of the Peer class specificly meant for clients
    /// </summary>
    public class Client : Peer
    {
        protected NetClient _client;

        public Client(String appIdentifier, INetworkGame game, ILogger logger)
            : base(appIdentifier, game, logger)
        {
            _client = new NetClient(_configuration);
        }

        public override void SendMessage(NetOutgoingMessage om, NetDeliveryMethod method = NetDeliveryMethod.UnreliableSequenced)
        {
            _client.SendMessage(om, method);
        }
    }
}
