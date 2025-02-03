using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Game.Server.Guppy
{
    public class ConfigureSimulationsSystem(ISimulationService simulationService) : ISceneSystem<ServerGameScene>
    {
        private readonly ISimulationService _simulationService = simulationService;

        [SequenceGroup<InitializeSystemSequenceGroupEnum>(InitializeSystemSequenceGroupEnum.PreInitialize)]
        public void Initialize(ServerGameScene scene)
        {
            this._simulationService.Create(VhId.Empty, StrategyTypeEnum.Lockstep);
        }
    }
}