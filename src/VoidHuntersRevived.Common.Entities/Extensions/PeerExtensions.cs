using Guppy.Network.Identity;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.Extensions
{
    public static class PeerExtensions
    {
        public static ParallelKey GetKey(this NetPeer peer)
        {
            return ParallelEntityTypes.Pilot.Create(peer.Id);
        }
    }
}
