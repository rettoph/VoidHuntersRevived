using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Common.Systems;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    internal sealed class AetherSystem : BasicSystem, ISimulationUpdateSystem
    {
        public void Update(ISimulation simulation, GameTime gameTime)
        {
            simulation.Aether.Step(gameTime.ElapsedGameTime);
        }
    }
}
