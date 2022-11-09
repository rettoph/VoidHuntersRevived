﻿using Guppy.Common;
using Guppy.MonoGame.Services;
using Guppy.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
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
        private readonly World _world;
        private readonly IInputService _inputs;
        private readonly IBus _bus;

        public ClientGameGuppy(World world, NetScope netScope, IBus bus, IInputService inputs, IGameComponentService components) : base(netScope, components)
        {
            _world = world;
            _inputs = inputs;
            _bus = bus;

            _inputs.Subscribe(_bus);
        }

        public void Dispose()
        {
            _inputs.Unsubscribe(_bus);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _world.Draw(gameTime);
        }
    }
}
