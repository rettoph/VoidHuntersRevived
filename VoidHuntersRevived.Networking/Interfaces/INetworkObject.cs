using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Networking.Interfaces
{
    public interface INetworkObject
    {
        /// <summary>
        /// A unique identifier for the current network object
        /// </summary>
        Int64 Id { get; }

        /// <summary>
        /// Update the current object from the data contained
        /// within a given NetIncomingMessage
        /// </summary>
        /// <param name="im"></param>
        void Read(NetIncomingMessage im);

        /// <summary>
        /// Write the current object data to a given
        /// NetOutgoingMessage
        /// </summary>
        /// <param name="om"></param>
        void Write(NetOutgoingMessage om);
    }
}
