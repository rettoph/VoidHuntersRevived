using Guppy.Core.Common.Attributes;
using Guppy.Engine.Common.Enums;
using Guppy.Game.Common;
using Guppy.Game.Common.Attributes;
using Guppy.Game.Common.Components;
using Guppy.Game.Common.Enums;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Game.Core.Components.Scene
{
    [AutoLoad]
    [SceneFilter<VoidHuntersGameScene>]
    [Sequence<InitializeSequence>(InitializeSequence.Initialize)]
    [Sequence<DrawSequence>(DrawSequence.Draw)]
    [Sequence<UpdateSequence>(UpdateSequence.Update)]
    internal class SimulationFrameComponent : SceneComponent, IGuppyDrawable, IGuppyUpdateable
    {
        private readonly ISimulationService _simulationService;

        public SimulationFrameComponent(ISimulationService simulationService)
        {
            _simulationService = simulationService;
        }

        public void Draw(GameTime gameTime)
        {
            _simulationService.Draw(gameTime);
        }

        public void Update(GameTime gameTime)
        {
            _simulationService.Update(gameTime);
        }
    }
}
