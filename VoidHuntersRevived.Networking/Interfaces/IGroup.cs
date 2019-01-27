using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Networking.Collections;
using VoidHuntersRevived.Networking.Enums;

namespace VoidHuntersRevived.Networking.Interfaces
{
    /// <summary>
    /// A network group is a collection of NetUsers
    /// and allows for easy self contained messaging
    /// </summary>
    public interface IGroup
    {
        Int64 Id { get; }

        IPeer Peer { get; }

        IDataHandler DataHandler { get; set; }

        UserCollection Users { get; }

        Queue<NetIncomingMessage> UnreadMessagesQueue { get; }

        NetOutgoingMessage CreateMessage();
        NetOutgoingMessage CreateMessage(MessageType type);

        void Update();

        void SendMessage(NetOutgoingMessage msg, NetDeliveryMethod method = NetDeliveryMethod.UnreliableSequenced, Int32 sequenceChannel = 0);
    }
}
