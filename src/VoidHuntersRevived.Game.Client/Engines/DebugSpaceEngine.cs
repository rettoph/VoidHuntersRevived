using Guppy.Attributes;
using Guppy.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Enums;
using VoidHuntersRevived.Common.Simulations;
using Microsoft.Xna.Framework;
using Svelto.ECS;
using VoidHuntersRevived.Common.Simulations.Engines;
using Microsoft.Xna.Framework.Graphics;
using Guppy.Resources.Providers;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Physics;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [Sequence<DrawEngineSequence>(DrawEngineSequence.PostDraw)]
    [SimulationTypeFilter(SimulationType.Predictive)]
    public class DebugSpaceEngine : BasicEngine, IStepEngine<GameTime>
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly SpriteFont _font;
        private readonly ISpace _space;

        public string name { get; } = nameof(DebugSpaceEngine);

        public DebugSpaceEngine(SpriteBatch spriteBatch, IResourceProvider resources, ISpace space)
        {
            _spriteBatch = spriteBatch;
            _font = resources.Get(Resources.SpriteFonts.Default) ?? throw new ArgumentException();
            _space = space;
        }

        public void Step(in GameTime param)
        {
            _spriteBatch.Begin();

            _spriteBatch.DrawString(_font, $"Enabled Bodies: " + _space.BodyCount(), new Vector2(5, 25), Color.White);

            _spriteBatch.End();
        }
    }
}
