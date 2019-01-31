using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using VoidHuntersRevived.Networking.Collections;
using VoidHuntersRevived.Networking.Enums;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Networking.Implementations
{
    public abstract class Peer : Group, IPeer
    {
        public Int64 UniqueIdentifier { get { return _peer.UniqueIdentifier; } }
        public GroupCollection Groups { get; protected set; }

        protected NetPeer _peer;
        protected NetPeerConfiguration _configuration;
        public ILogger Logger { get; private set; }
        private NetOutgoingMessage _om;

        public Peer(String appIdentifier, INetworkGame game, ILogger logger)
            : base(-1, null)
        {
            Logger = logger;

            _configuration = new NetPeerConfiguration(appIdentifier);
            _configuration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            _configuration.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            //_configuration.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
            _configuration.ConnectionTimeout = 10000;
        }

        #region IPeer Implementation
        /// <summary>
        /// Start the internal peer
        /// </summary>
        public void Start()
        {
            Logger.LogDebug("Starting peer...");
            _peer.Start();
        }
        #endregion

        #region IGroup Implementation
        public override NetOutgoingMessage CreateMessage(String messageType)
        {
            _om = this.CreateMessage(MessageTarget.Peer);
            _om.Write(messageType);

            return _om;
        }
        public NetOutgoingMessage CreateMessage(MessageTarget target)
        {
            _om = _peer.CreateMessage();
            _om.Write((Byte)target);

            return _om;
        }
        #endregion
    }
}
