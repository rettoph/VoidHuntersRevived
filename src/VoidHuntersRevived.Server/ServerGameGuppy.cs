using Guppy.Common;
using Guppy.MonoGame.Services;
using Guppy.Network;
using Guppy.Network.Identity;
using Guppy.Network.Identity.Services;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Server
{
    public sealed class ServerGameGuppy : GameGuppy
    {
        public ServerGameGuppy(World world, NetScope netScope, ITickService ticks, IBus bus) : base(world, netScope, ticks, bus)
        {
        }
    }
}
