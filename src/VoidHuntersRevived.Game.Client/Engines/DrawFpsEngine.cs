using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.Collections;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Client.Messages;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    internal class DrawFpsEngine : BasicEngine, IStepEngine<GameTime>,
        ISubscriber<Input_Toggle_FPS>
    {
        private bool _enabled;

        public string name { get; } = nameof(DrawFpsEngine);

        private readonly SpriteBatch _spriteBatch;
        private readonly SpriteFont _font;
        private Buffer<double> _sampleBuffer;
        private double _sampleSum;

        public DrawFpsEngine(SpriteBatch spriteBatch, IResourceProvider resources)
        {
            _spriteBatch = spriteBatch;
            _font = resources.Get(Resources.Fonts.Default) ?? throw new ArgumentException();
            _sampleBuffer = new Buffer<double>(500);
            _sampleSum = 0;
            _enabled = true;
        }

        public void Step(in GameTime _param)
        {
            if(!_enabled)
            {
                return;
            }

            _sampleSum += _param.ElapsedGameTime.TotalSeconds;
            _sampleBuffer.Add(_param.ElapsedGameTime.TotalSeconds, out double removed);
            _sampleSum -= removed;

            var fps = 500.0 / _sampleSum;

            _spriteBatch.Begin();

            _spriteBatch.DrawString(_font, $"FPS: {fps.ToString("#,##0")}", Vector2.One * 5, Color.White);

            _spriteBatch.End();
        }

        public void Process(in Guid messageId, in Input_Toggle_FPS message)
        {
            _enabled = !_enabled;
        }
    }
}
