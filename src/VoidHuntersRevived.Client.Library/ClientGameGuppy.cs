using Guppy.Common;
using Guppy.MonoGame.Services;
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
    public sealed class ClientGameGuppy : GameGuppy, IDisposable
    {
        private readonly IInputService _inputs;
        private readonly IBus _bus;

        public ClientGameGuppy(World world, NetScope netScope, ITickService ticks, IBus bus, IInputService inputs) : base(world, netScope, ticks, bus)
        {
            _inputs = inputs;
            _bus = bus;

            _inputs.Subscribe(_bus);
        }

        public void Dispose()
        {
            _inputs.Unsubscribe(_bus);
        }
    }
}
