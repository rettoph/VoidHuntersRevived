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
        private readonly ISimulationService _simluations;

        public SimulationFrameComponent(ISimulationService simluations)
        {
            _simluations = simluations;
        }

        public void Draw(GameTime gameTime)
        {
            _simluations.Draw(gameTime);
        }

        public void Update(GameTime gameTime)
        {
            _simluations.Update(gameTime);
        }
    }
}
