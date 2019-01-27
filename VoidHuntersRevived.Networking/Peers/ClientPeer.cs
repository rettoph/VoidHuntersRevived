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

namespace VoidHuntersRevived.Networking.Peers
{
    /// <summary>
    /// Implementation of the Peer class specificly meant for clients
    /// </summary>
    public class ClientPeer : Peer
    {
        protected NetClient _client;
        private NetIncomingMessage _im;

        public ClientPeer(String appIdentifier, INetworkGame game, ILogger logger)
            : base(appIdentifier, game, logger)
        {
            _client = new NetClient(_configuration);
            _peer = _client;

            // Create the new group collection
            this.Groups = new GroupCollection(this, typeof(ClientGroup));
        }

        /// <summary>
        /// Attempt to connect to an external server at a given address
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public void Connect(String host, Int32 port, NetOutgoingMessage hail)
        {
            _client.Connect(host, port, hail);
        }

        public override void Update()
        {
            while ((_im = _peer.ReadMessage()) != null)
            { // Read any new incoming messages
                _logger.LogInformation(_im.MessageType.ToString());

                switch (_im.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        this.HandleData(_im);
                        break;
                }
            }

            // Update the underlying group
            base.Update();
        }

        #region MessageType Handlers
        private void HandleData(NetIncomingMessage im)
        {
            switch ((MessageTarget)im.ReadByte())
            {
                case MessageTarget.Peer:
                    this.UnreadMessagesQueue.Enqueue(im);
                    break;
                case MessageTarget.Group:
                    this.Groups.GetById(im.ReadInt64())
                        .UnreadMessagesQueue
                        .Enqueue(im);
                    break;
            }
        }

        protected override void HandleUserJoined(NetIncomingMessage im)
        {
            throw new NotImplementedException();
        }

        protected override void HandleUserLeft(NetIncomingMessage im)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region SendMessage Methods
        public override void SendMessage(NetOutgoingMessage msg, NetDeliveryMethod method = NetDeliveryMethod.UnreliableSequenced, int sequenceChannel = 0)
        {
            _client.SendMessage(msg, method, sequenceChannel);
        }
        #endregion
    }
}
