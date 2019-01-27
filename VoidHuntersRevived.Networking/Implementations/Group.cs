using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;
using VoidHuntersRevived.Networking.Collections;
using VoidHuntersRevived.Networking.Enums;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Networking.Implementations
{
    public abstract class Group : IGroup
    {
        public Int64 Id { get; protected set; }
        public IPeer Peer { get; protected set; }
        public IDataHandler DataHandler { get; set; }
        public UserCollection Users { get; protected set; }
        public Queue<NetIncomingMessage> UnreadMessagesQueue { get; protected set; }

        private NetOutgoingMessage _om;
        private NetIncomingMessage _im;


        public Group(Int64 id, IPeer peer)
        {
            this.Id = id;
            this.Peer = peer;
            this.Users = new UserCollection();
            this.UnreadMessagesQueue = new Queue<NetIncomingMessage>();
        }

        public virtual void Update()
        { // Read and handle any unread messages
            if(this.DataHandler != null)
                while (this.UnreadMessagesQueue.Count > 0)
                {
                    _im = this.UnreadMessagesQueue.Dequeue();

                    switch ((MessageType)_im.ReadByte())
                    {
                        case MessageType.Data:
                            break;
                        case MessageType.UserJoined:
                            this.HandleUserJoined(_im);
                            break;
                        case MessageType.UserLeft:
                            this.HandleUserLeft(_im);
                            break;
                    }
                }
        }

        public virtual NetOutgoingMessage CreateMessage()
        {
            return this.CreateMessage(MessageType.Data);
        }
        public virtual NetOutgoingMessage CreateMessage(MessageType type)
        {
            _om = this.Peer.CreateMessage(MessageTarget.Group);
            _om.Write(this.Id);
            _om.Write((Byte)type);

            return _om;
        }

        public abstract void SendMessage(NetOutgoingMessage msg, NetDeliveryMethod method = NetDeliveryMethod.UnreliableSequenced, int sequenceChannel = 0);

        #region MessageType Handlers
        protected abstract void HandleUserJoined(NetIncomingMessage im);
        protected abstract void HandleUserLeft(NetIncomingMessage im);
        #endregion
    }
}
