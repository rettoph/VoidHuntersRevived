using Guppy;
using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Enums;
using Guppy.Game;
using Guppy.Game.Common;
using Guppy.Game.Common.Enums;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Game.Components.Guppy
{
    [AutoLoad]
    [GuppyFilter<GameGuppy>]
    [Sequence<InitializeSequence>(InitializeSequence.PostInitialize)]
    [Sequence<UpdateSequence>(UpdateSequence.Update)]
    [Sequence<DrawSequence>(DrawSequence.Draw)]
    internal class SimulationComponent : IGuppyComponent, IGuppyUpdateable, IGuppyDrawable
    {
        private readonly ISimulationService _simulations;

        public SimulationComponent(ISimulationService simulations)
        {
            _simulations = simulations;
        }

        public void Initialize(IGuppy guppy)
        {
            _simulations.Initialize();
        }

        public void Update(GameTime gameTime)
        {
            _simulations.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            _simulations.Draw(gameTime);
        }
    }
}
