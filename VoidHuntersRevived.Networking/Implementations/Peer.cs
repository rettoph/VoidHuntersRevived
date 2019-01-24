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

        private NetOutgoingMessage _om;

        public Peer(String appIdentifier, INetworkGame game, ILogger logger)
        {
            this.Id = Guid.NewGuid();

            _logger = logger;

            _configuration = new NetPeerConfiguration(appIdentifier);
            _configuration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            _configuration.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
        }

        public void Start()
        {
            _logger.LogDebug("Starting peer...");
            _peer.Start();
        }

        public NetOutgoingMessage CreateMessage()
        {
            _om = _peer.CreateMessage();
            _om.Write((Byte)MessageTarget.Peer);

            return _om;
        }

        public abstract void SendMessage(NetOutgoingMessage om, NetDeliveryMethod method = NetDeliveryMethod.UnreliableSequenced);

        public void SendMessage(NetOutgoingMessage om, IUser user, NetDeliveryMethod method = NetDeliveryMethod.UnreliableSequenced)
        {
            throw new NotImplementedException();
        }
    }
}
