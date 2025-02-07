using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Game.Client.Components.Scene
{
    public class ConfigureSimulationsSystem(ISimulationService simulationService) : ISceneSystem
    {
        private readonly ISimulationService _simulationService = simulationService;

        [SequenceGroup<InitializeSequenceGroupEnum>(InitializeSequenceGroupEnum.PreInitialize)]
        public void Initialize()
        {
            this._simulationService.Create(VhId.Empty, StrategyTypeEnum.Predictive, StrategyTypeEnum.Lockstep);
        }
    }
}