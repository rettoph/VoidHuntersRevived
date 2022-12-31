using Guppy.Common;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Games;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.Simulations.Systems.Lockstep
{
    internal sealed class LockstepAetherSystem : LockstepUpdateSystem
    {
        private readonly ISimulationService _simulations;

        public LockstepAetherSystem(ISimulationService simulations)
        {
            _simulations = simulations;
        }

        protected override void Update(GameTime gameTime)
        {
            _simulations[SimulationType.Lockstep].Aether.Step(gameTime.ElapsedGameTime);
        }
    }
}
