using Guppy.Core.Common.Attributes;
using Guppy.Engine.Common.Enums;
using Guppy.Game.Common;
using Guppy.Game.Common.Attributes;
using Guppy.Game.Common.Components;
using Guppy.Game.Common.Enums;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Domain.Simulations.Components.Guppy
{
    [AutoLoad]
    [SceneFilter<IVoidHuntersGameScene>]
    [Sequence<InitializeSequence>(InitializeSequence.PostInitialize)]
    [Sequence<UpdateSequence>(UpdateSequence.Update)]
    [Sequence<DrawSequence>(DrawSequence.Draw)]
    internal class SimulationComponent : SceneComponent, IGuppyUpdateable, IGuppyDrawable
    {
        private readonly ISimulationService _simulations;

        public SimulationComponent(ISimulationService simulations)
        {
            _simulations = simulations;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _simulations.Initialize();
        }

        public void Update(GameTime gameTime)
        {
            // _simulations.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            // _simulations.Draw(gameTime);
        }
    }
}
