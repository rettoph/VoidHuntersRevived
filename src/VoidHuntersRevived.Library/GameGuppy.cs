using Guppy.Attributes;
using Guppy.Common;
using Guppy.MonoGame;
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
using VoidHuntersRevived.Library.Games;
using VoidHuntersRevived.Library.Messages;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Simulations;

namespace VoidHuntersRevived.Library
{
    public class GameGuppy : FrameableGuppy
    {
        protected virtual SimulationType SimulationFlags => SimulationType.Lockstep;
        public readonly World World;
        public readonly NetScope NetScope;
        public readonly IBus Bus;
        public readonly ISimulationService Simulations;

        public GameGuppy(
            Lazy<World> world,
            Lazy<IBus> bus,
            NetScope netScope,
            ISimulationService simulations,
            IGameComponentService components) : base(components)
        {
            this.NetScope = netScope;
            this.Simulations = simulations;

            this.Simulations.Initialize(this.SimulationFlags);
            this.World = world.Value;
            this.Bus = bus.Value;

            this.Bus.Initialize();

            this.NetScope.Start(0);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.World.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Bus.Flush();

            this.World.Update(gameTime);

            this.Simulations.Update(gameTime);
        }
    }
}
