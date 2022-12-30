using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Simulations
{
    public interface ISimulation
    {
        SimulationType Type { get; }
        AetherWorld Aether { get; }

        internal void Update(GameTime gameTime);
    }
}
