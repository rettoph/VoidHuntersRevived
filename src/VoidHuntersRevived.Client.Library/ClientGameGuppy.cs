using Guppy.Common;
using Guppy.Network;
using Microsoft.Extensions.DependencyInjection;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Client.Library
{
    public sealed class ClientGameGuppy : GameGuppy
    {
        public ClientGameGuppy(World world, NetScope netScope, ITickService ticks, IBus bus) : base(world, netScope, ticks, bus)
        {
        }
    }
}
