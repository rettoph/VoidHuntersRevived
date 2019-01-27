using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Networking.Interfaces
{
    /// <summary>
    /// A data handler is used to handle incoming data on certain groups.
    /// Allowing for an easy interface to incoming messages
    /// </summary>
    public interface IDataHandler
    {
        /// <summary>
        /// Triggered when the group recieves new data
        /// </summary>
        /// <param name="data"></param>
        void HandleData(NetIncomingMessage data);

        /// <summary>
        /// Triggered when a specific user joins the group
        /// </summary>
        /// <param name="user"></param>
        /// <param name="connection"></param>
        void HandleUserJoined(IUser user, NetConnection connection = null);

        /// <summary>
        /// Triggered when a specific user leaves the group
        /// </summary>
        /// <param name=""></param>
        /// <param name="connection"></param>
        void HandleUserLeft(IUser user, NetConnection connection = null);
    }
}
