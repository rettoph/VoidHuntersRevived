using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Guppy.Core.Common.Systems;
using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Game.Server.Guppy
{
    public class ConfigureSimulationsSystem(ISimulationService simulationService) : ISceneSystem, IInitializeSystem
    {
        private readonly ISimulationService _simulationService = simulationService;

        [SequenceGroup<InitializeSequenceGroupEnum>(InitializeSequenceGroupEnum.PreInitialize)]
        public void Initialize()
        {
            this._simulationService.Create(VhId.Empty, StrategyTypeEnum.Lockstep);
        }
    }
}