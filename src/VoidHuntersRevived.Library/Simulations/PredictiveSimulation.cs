using Guppy.Common;
using Guppy.Common.Providers;
using Guppy.ECS.Providers;
using Microsoft.Extensions.Options;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Constants;
using VoidHuntersRevived.Library.Maps;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Simulations;

namespace VoidHuntersRevived.Library.Games
{
    internal sealed class PredictiveSimulation : Simulation
    {
        public override SimulationType Type => SimulationType.Predictive;

        public override AetherWorld Aether { get; } = new AetherWorld(Vector2.Zero);

        public PredictiveSimulation(Lazy<World> world, SimulatedEntityIdService simulatedEntities) : base(world, simulatedEntities)
        {
        }

        protected override void Update(GameTime gameTime)
        {
            this.Aether.Step(gameTime.ElapsedGameTime);
        }
    }
}
