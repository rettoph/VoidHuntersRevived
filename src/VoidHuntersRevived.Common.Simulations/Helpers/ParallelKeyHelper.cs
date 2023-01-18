using Guppy.Network.Identity;
using Guppy.Network.Peers;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations.Helpers
{
    public static class ParallelKeyHelper
    {
        public const string PilotKeyType = "pilot";

        public static ParallelKey GetPilotKey(int userId)
        {
            return ParallelKey.From(PilotKeyType, userId);
        }
    }
}
