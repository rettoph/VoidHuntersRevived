using Guppy.DependencyInjection;
using Guppy.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Client.Library.Effects
{
    public class GaussianBlurFilter
    {
        #region Private Fields
        private Effect _effect;
        private GraphicsDevice _graphics;
        private SpriteBatch _spriteBatch;
        private RenderTargetBinding[] _targets;
        private Boolean _started;

        /// <summary>
        /// The target "to be blurred"
        /// </summary>
        private RenderTarget2D _source;

        /// <summary>
        /// The target "to blur"
        /// </summary>
        private RenderTarget2D _target;

        private RenderTarget2D _temp;
        #endregion

        #region Public Properties
        public Point Resolution
        {
            set
            {
                _effect.Parameters["InverseResolution"].SetValue(new Vector2(1f / value.X, 1f / value.Y));

                _source?.Dispose();
                _source = new RenderTarget2D(_graphics, value.X, value.Y);

                _target?.Dispose();
                _target = new RenderTarget2D(_graphics, value.X, value.Y);
            }
        }

        public Single StreakLength
        {
            get => _effect.Parameters["StreakLength"].GetValueSingle();
            set => _effect.Parameters["StreakLength"].SetValue(value);
        }

        public Int32 Passes { get; set; }
        #endregion

        #region Constructors
        public GaussianBlurFilter(ServiceProvider provider) : this(provider.GetService<ContentService>(), provider.GetService<GraphicsDevice>())
        {

        }
        public GaussianBlurFilter(ContentService content, GraphicsDevice graphics)
        {
            _effect = content.Get<Effect>("effect:blur");
            _graphics = graphics;
            _spriteBatch = new SpriteBatch(_graphics);
        }
        #endregion

        #region Helper Methods
        public void Start()
        {
            if (_started)
                throw new Exception("Unable to start blur filter more than once.");

            _targets = _graphics.GetRenderTargets();
            _graphics.SetRenderTarget(_source);
            _graphics.Clear(Color.Transparent);
            _started = true;
        }

        public void End()
        {
            if (!_started)
                throw new Exception("Unable to end blur filter before started.");

            for (Int32 i=0; i<this.Passes; i++)
            {
                _graphics.SetRenderTarget(_target);
                _graphics.Clear(Color.Transparent);
            
                _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            
                foreach(EffectPass pass in _effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    _spriteBatch.Draw(
                        texture: _source,
                        position: Vector2.Zero,
                        sourceRectangle: _source.Bounds,
                        color: Color.White,
                        rotation: 0,
                        origin: Vector2.Zero,
                        scale: 1f,
                        effects: SpriteEffects.None,
                        layerDepth: 0);
                }

                _spriteBatch.End();

                _temp = _source;
                _source = _target;
                _target = _temp;
            }
            
            // Draw the finalized blur to the primary render target
            _graphics.SetRenderTargets(_targets);
            
            _spriteBatch.Begin();
            _spriteBatch.Draw(
                texture: _source,
                position: Vector2.Zero,
                sourceRectangle: _source.Bounds,
                color: Color.White,
                rotation: 0,
                origin: Vector2.Zero,
                scale: 1f,
                effects: SpriteEffects.None,
                layerDepth: 0);
            _spriteBatch.End();
            _started = false;
        }
        #endregion
    }
}
