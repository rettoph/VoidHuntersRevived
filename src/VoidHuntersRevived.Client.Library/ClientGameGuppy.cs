using Guppy.Common;
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
using VoidHuntersRevived.Library.Games;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Simulations;

namespace VoidHuntersRevived.Client.Library
{
    public sealed class ClientGameGuppy : GameGuppy
    {
        protected override SimulationType SimulationFlags => base.SimulationFlags | SimulationType.Predictive;
        public ClientGameGuppy(
            Lazy<World> world, 
            Lazy<IBus> bus, 
            NetScope netScope,
            ISimulationService simulations,
            IGameComponentService components) : base(world, bus, netScope, simulations, components)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
