using Guppy.Core.Common.Attributes;
using Guppy.Game.Common;
using Guppy.Game.Common.Attributes;
using Guppy.Game.Common.Components;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Game.Core.Components.Scene
{
    [AutoLoad]
    [SceneFilter<VoidHuntersGameScene>]
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
