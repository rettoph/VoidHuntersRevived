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
        NetOutgoingMessage CreateMessage();
    }
}
