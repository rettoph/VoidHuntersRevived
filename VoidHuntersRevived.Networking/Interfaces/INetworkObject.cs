using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Networking.Interfaces
{
    public interface INetworkObject
    {
        Int64 Id { get; }

        void Read(NetIncomingMessage im);
        void Write(NetOutgoingMessage om);
    }
}
