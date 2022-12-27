using Guppy.Common;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Games;

namespace VoidHuntersRevived.Library.Simulations.Systems.Lockstep
{
    internal sealed class LockstepAetherSystem : LockstepUpdateSystem
    {
        private readonly LockstepSimulation _simulation;

        public LockstepAetherSystem(LockstepSimulation simulation)
        {
            _simulation = simulation;
        }

        protected override void Update(GameTime gameTime)
        {
            _simulation.Aether.Step(gameTime.ElapsedGameTime);
        }
    }
}
