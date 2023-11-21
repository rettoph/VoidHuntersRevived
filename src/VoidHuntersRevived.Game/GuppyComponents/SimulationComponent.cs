using Guppy;
using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Enums;
using Guppy.MonoGame;
using Guppy.MonoGame.Common;
using Guppy.MonoGame.Common.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Game.GuppyComponents
{
    [AutoLoad]
    [GuppyFilter<GameGuppy>]
    [Sequence<InitializeSequence>(InitializeSequence.PostInitialize)]
    [Sequence<UpdateSequence>(UpdateSequence.Update)]
    [Sequence<DrawSequence>(DrawSequence.Draw)]
    internal class SimulationComponent : IGuppyComponent, IUpdateableComponent, IDrawableComponent
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
