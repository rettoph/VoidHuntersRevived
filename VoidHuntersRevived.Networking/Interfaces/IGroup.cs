using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Networking.Interfaces
{
    /// <summary>
    /// A network group is a collection of NetUsers
    /// and allows for easy self contained messaging
    /// </summary>
    public interface IGroup
    {
        /// <summary>
        /// A unique id that should be shared between the server and client
        /// linking 2 groups together
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Create a new message that will be recieved and handled by
        /// any recieving connections version of this group.
        /// </summary>
        /// <returns></returns>
        NetOutgoingMessage CreateMessage();

        /// <summary>
        /// Send a message to all connected users
        /// </summary>
        /// <param name="om"></param>
        void SendMessage(NetOutgoingMessage om, NetDeliveryMethod method = NetDeliveryMethod.UnreliableSequenced);

        /// <summary>
        /// Send a message to a specific user
        /// </summary>
        /// <param name="om"></param>
        /// <param name="user"></param>
        /// <param name="method"></param>
        void SendMessage(NetOutgoingMessage om, IUser user, NetDeliveryMethod method = NetDeliveryMethod.UnreliableSequenced);
    }
}
