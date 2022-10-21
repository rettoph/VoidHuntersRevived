using Guppy.Common;
using Guppy.MonoGame;
using Guppy.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library
{
    public class GameGuppy : FrameableGuppy
    {
        private readonly ITickService _ticks;
        private readonly IBus _bus;

        public readonly NetScope NetScope;

        public IBus Bus => _bus;

        public GameGuppy(
            World world,
            NetScope netScope,
            ITickService ticks,
            IBus bus) : base(world)
        {
            _ticks = ticks;
            _bus = bus;

            this.NetScope = netScope;

            this.NetScope.Start(0);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            _bus.Flush();

            _ticks.Update(gameTime);

            base.Update(gameTime);
        }
    }
}
