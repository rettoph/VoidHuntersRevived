using Guppy.Core.Common.Attributes;
using Guppy.Engine.Common.Enums;
using Guppy.Game.Common.Attributes;
using Guppy.Game.Common.Components;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Game.Client.Components.Guppy
{
    [AutoLoad]
    [SceneFilter<LocalGameScene>]
    [Sequence<InitializeSequence>(InitializeSequence.PreInitialize)]
    internal class ConfigureSimulationsComponent : SceneComponent
    {
        private readonly ISimulationService _simluations;

        public ConfigureSimulationsComponent(ISimulationService simluations)
        {
            _simluations = simluations;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _simluations.Create(VhId.Empty, StrategyTypeEnum.Predictive, StrategyTypeEnum.Lockstep);
        }
    }
}
