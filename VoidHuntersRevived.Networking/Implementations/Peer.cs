using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using VoidHuntersRevived.Networking.Enums;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Networking.Implementations
{
    public abstract class Peer : IPeer
    {
        public Guid Id { get; private set; }

        protected NetPeer _peer;
        protected NetPeerConfiguration _configuration;
        protected ILogger _logger;

        public Peer(String appIdentifier, INetworkGame game, ILogger logger)
        {
            this.Id = Guid.NewGuid();

            _logger = logger;

            _configuration = new NetPeerConfiguration(appIdentifier);
            _configuration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            _configuration.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
        }

        #region IPeer Implementation
        /// <summary>
        /// Start the internal peer
        /// </summary>
        public void Start()
        {
            _logger.LogDebug("Starting peer...");
            _peer.Start();
        }

        public abstract void Update();
        #endregion

        #region IGroup Implementation
        public NetOutgoingMessage CreateMessage()
        {
            return _peer.CreateMessage();
        }
        #endregion
    }
}
