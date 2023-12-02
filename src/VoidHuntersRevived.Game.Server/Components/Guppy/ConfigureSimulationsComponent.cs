using Guppy;
using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Game.Server.Guppy
{
    [AutoLoad]
    [GuppyFilter<ServerGameGuppy>]
    [Sequence<InitializeSequence>(InitializeSequence.PreInitialize)]
    internal class ConfigureSimulationsComponent : IGuppyComponent
    {
        private readonly ISimulationService _simluations;

        public ConfigureSimulationsComponent(ISimulationService simluations)
        {
            _simluations = simluations;
        }

        public void Initialize(IGuppy guppy)
        {
            _simluations.Configure(SimulationType.Lockstep);
        }
    }
}
