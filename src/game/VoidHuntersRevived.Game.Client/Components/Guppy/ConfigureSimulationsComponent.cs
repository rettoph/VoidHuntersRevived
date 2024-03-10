using Guppy;
using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Enums;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Game.Client.Components.Guppy
{
    [AutoLoad]
    [GuppyFilter<LocalGameGuppy>]
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
            _simluations.Configure(SimulationType.Lockstep | SimulationType.Predictive);
        }
    }
}
