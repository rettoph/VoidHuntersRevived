using Guppy.Core.Common.Attributes;
using Guppy.Engine.Common.Enums;
using Guppy.Game.Common;
using Guppy.Game.Common.Attributes;
using Guppy.Game.Common.Components;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Game.Server.Guppy
{
    [AutoLoad]
    [SceneFilter<ServerGameScene>]
    internal class ConfigureSimulationsComponent : ISceneComponent<IScene>
    {
        private readonly ISimulationService _simulationService;

        public ConfigureSimulationsComponent(ISimulationService simulationService)
        {
            _simulationService = simulationService;
        }

        [SequenceGroup<InitializeComponentSequenceGroup>(InitializeComponentSequenceGroup.PreInitialize)]
        public void Initialize(IScene scene)
        {
            _simulationService.Create(VhId.Empty, StrategyTypeEnum.Lockstep);
        }
    }
}
