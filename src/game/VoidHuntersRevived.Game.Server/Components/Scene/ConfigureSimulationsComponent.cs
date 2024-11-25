using Guppy.Core.Common.Attributes;
using Guppy.Engine.Common.Enums;
using Guppy.Game.Common.Components;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Game.Server.Guppy
{
    internal class ConfigureSimulationsComponent(ISimulationService simulationService) : ISceneComponent<ServerGameScene>
    {
        private readonly ISimulationService _simulationService = simulationService;

        [SequenceGroup<InitializeComponentSequenceGroup>(InitializeComponentSequenceGroup.PreInitialize)]
        public void Initialize(ServerGameScene scene)
        {
            _simulationService.Create(VhId.Empty, StrategyTypeEnum.Lockstep);
        }
    }
}
