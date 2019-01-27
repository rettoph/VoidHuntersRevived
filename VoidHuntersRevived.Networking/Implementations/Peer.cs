﻿using System;
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
        public GroupCollection Groups { get; protected set; }

        protected NetPeer _peer;
        protected NetPeerConfiguration _configuration;
        protected ILogger _logger;
        private NetOutgoingMessage _om;

        public Peer(String appIdentifier, INetworkGame game, ILogger logger)
            : base(-1, null)
        {
            _logger = logger;

            _configuration = new NetPeerConfiguration(appIdentifier);
            _configuration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            _configuration.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
            _configuration.ConnectionTimeout = 10;
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
        #endregion

        #region IGroup Implementation
        public override NetOutgoingMessage CreateMessage()
        {
            return this.CreateMessage(MessageType.Data);
        }
        public override NetOutgoingMessage CreateMessage(MessageType type)
        {
            _om =  this.CreateMessage(MessageTarget.Peer);
            _om.Write((Byte)type);

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