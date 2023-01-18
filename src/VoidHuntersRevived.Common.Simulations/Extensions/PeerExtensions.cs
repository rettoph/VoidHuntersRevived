using Guppy.Network.Identity;
using Guppy.Network.Peers;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Helpers;

namespace VoidHuntersRevived.Common.Simulations.Extensions
{
    public static class PeerExtensions
    {
        public static ParallelKey GetPilotKey(this NetPeer peer)
        {
            return ParallelKeyHelper.GetPilotKey(peer.Id);
        }
    }
}
