using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.Extensions;
using Guppy.Common.Implementations;
using Guppy.Common.Providers;
using Guppy.ECS.Providers;
using Microsoft.Extensions.Options;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Simulations
{
    public abstract class Simulation : ISimulation
    {
        public abstract SimulationType Type { get; }
        public abstract AetherWorld Aether { get; }

        protected abstract void Update(GameTime gameTime);

        void ISimulation.Update(GameTime gameTime)
        {
            this.Update(gameTime);
        }
    }
}
