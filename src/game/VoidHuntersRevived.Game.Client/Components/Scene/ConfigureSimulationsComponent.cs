using Guppy.Core.Common.Attributes;
using Guppy.Engine.Common.Enums;
using Guppy.Game.Common.Components;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Game.Client.Components.Scene
{
    internal class ConfigureSimulationsComponent(ISimulationService simulationService) : ISceneComponent<LocalGameScene>
    {
        private readonly ISimulationService _simulationService = simulationService;

        [SequenceGroup<InitializeComponentSequenceGroup>(InitializeComponentSequenceGroup.PreInitialize)]
        public void Initialize(LocalGameScene scene)
        {
            this._simulationService.Create(VhId.Empty, StrategyTypeEnum.Predictive, StrategyTypeEnum.Lockstep);
        }
    }
}