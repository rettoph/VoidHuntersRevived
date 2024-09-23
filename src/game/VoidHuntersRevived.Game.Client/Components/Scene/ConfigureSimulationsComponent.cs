using Guppy.Core.Common.Attributes;
using Guppy.Engine.Common.Enums;
using Guppy.Game.Common.Attributes;
using Guppy.Game.Common.Components;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Game.Client.Components.Scene
{
    [AutoLoad]
    [SceneFilter<LocalGameScene>]
    [SequenceGroup<InitializeSequence>(InitializeSequence.PreInitialize)]
    internal class ConfigureSimulationsComponent : SceneComponent
    {
        private readonly ISimulationService _simulationService;

        public ConfigureSimulationsComponent(ISimulationService simulationService)
        {
            _simulationService = simulationService;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _simulationService.Create(VhId.Empty, StrategyTypeEnum.Predictive, StrategyTypeEnum.Lockstep);
        }
    }
}
