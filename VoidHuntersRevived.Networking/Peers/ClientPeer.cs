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
    public class ClientPeer : Peer
    {
        protected NetClient _client;

        public ClientPeer(String appIdentifier, INetworkGame game, ILogger logger)
            : base(appIdentifier, game, logger)
        {
            _client = new NetClient(_configuration);
            _peer = _client;
        }

        /// <summary>
        /// Attempt to connect to an external server at a given address
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public void Connect(String host, Int32 port, IUser user)
        {
            _client.Connect(host, port);
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }
    }
}
