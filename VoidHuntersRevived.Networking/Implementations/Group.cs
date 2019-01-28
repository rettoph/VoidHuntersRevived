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
    public abstract class Group : IGroup
    {
        public Int64 Id { get; protected set; }
        public IPeer Peer { get; protected set; }
        public UserCollection Users { get; protected set; }
        public Dictionary<String, Action<NetIncomingMessage>> MessageTypeHandlers { get; private set; }
        public Queue<NetIncomingMessage> UnreadMessagesQueue { get; protected set; }

        private NetOutgoingMessage _om;
        private NetIncomingMessage _im;
        private String _messageType;


        public Group(Int64 id, IPeer peer)
        {
            this.Id = id;
            this.Peer = peer;
            this.Users = new UserCollection();
            this.MessageTypeHandlers = new Dictionary<String, Action<NetIncomingMessage>>();
            this.UnreadMessagesQueue = new Queue<NetIncomingMessage>();
        }

        public virtual void Update()
        { // Read and handle any unread messages
            while(this.UnreadMessagesQueue.Count > 0)
            {
                _im = this.UnreadMessagesQueue.Dequeue();
                _messageType = _im.ReadString();

                if (this.MessageTypeHandlers.ContainsKey(_messageType))
                    this.MessageTypeHandlers[_messageType](_im);
                else
                    this.Peer.Logger.LogWarning($"Unhandled incoming message type: '{_messageType}'");
            }
        }

        public virtual NetOutgoingMessage CreateMessage(String messageType)
        {
            _om = this.Peer.CreateMessage(MessageTarget.Group);
            _om.Write(this.Id);
            _om.Write(messageType);

            return _om;
        }

        public abstract void SendMessage(NetOutgoingMessage msg, NetDeliveryMethod method = NetDeliveryMethod.UnreliableSequenced, int sequenceChannel = 0);

        #region MessageType Handlers
        protected abstract void HandleUserJoined(NetIncomingMessage im);
        protected abstract void HandleUserLeft(NetIncomingMessage im);
        #endregion
    }
}
