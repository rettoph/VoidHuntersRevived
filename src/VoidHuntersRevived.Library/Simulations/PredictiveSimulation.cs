using Guppy.Common;
using Guppy.Common.Providers;
using Guppy.ECS.Providers;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Constants;
using VoidHuntersRevived.Library.Simulations;

namespace VoidHuntersRevived.Library.Games
{
    public sealed class PredictiveSimulation : Simulation
    {
        public override SimulationType Type => SimulationType.Predictive;

        public override AetherWorld Aether => throw new NotImplementedException();

        public PredictiveSimulation() : base()
        {
        }
    }
}
