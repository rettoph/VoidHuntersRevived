using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Networking.Interfaces
{
    /// <summary>
    /// A game designed to contain a peer objects.
    /// </summary>
    public interface INetworkGame : IGame
    {
        IPeer Peer { get; }
    }
}
