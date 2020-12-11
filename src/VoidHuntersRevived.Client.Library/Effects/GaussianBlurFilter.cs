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

        /// <summary>
        /// The placeholder field storing all recieved <see cref="BlurAmount"/>
        /// values.
        /// </summary>
        private Single _blurAmount = 2f;

        /// <summary>
        /// Sample weights to be applied into 
        /// the effect.
        /// </summary>
        private Single[] _sampleWeights;

        /// <summary>
        /// The number of samples within the effect.
        /// </summary>
        private Int32 _sampleCount;

        /// <summary>
        /// The screen's inverse resolution.
        /// Used when calculating the offset values.
        /// </summary>
        Vector2 _inverseResolution;

        /// <summary>
        /// The effect params to be updated
        /// </summary>
        private EffectParameter _blurWeightsParam;
        private EffectParameter _blurOffsetsParam;
        #endregion

        #region Public Properties
        /// <summary>
        /// amount to blur. A range of 0.5 - 6 works well. Defaults to 2.
        /// </summary>
        /// <value>The blur amount.</value>
        public float BlurAmount
        {
            get => _blurAmount;
            set
            {
                if (_blurAmount != value)
                {
                    // avoid 0 which will get is NaNs
                    if (value == 0)
                        value = 0.001f;

                    _blurAmount = value;
                    CalculateSampleWeights();
                }
            }
        }

        public Point Resolution
        {
            set
            {
                _inverseResolution = new Vector2(1f / value.X, 1f / value.Y);

                _source?.Dispose();
                _source = new RenderTarget2D(_graphics, value.X, value.Y);

                _target?.Dispose();
                _target = new RenderTarget2D(_graphics, value.X, value.Y);

                this.SetBlurOffsetParameter();
            }
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

            _blurWeightsParam = _effect.Parameters["_sampleWeights"];
            _blurOffsetsParam = _effect.Parameters["_sampleOffsets"];

            // Look up how many samples our gaussian blur effect supports.
            _sampleCount = _blurWeightsParam.Elements.Count;
            _sampleWeights = new Single[_sampleCount];
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

                var temp = _source;
                _source = _target;
                _target = temp;
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

        #region Helper Methods
        /// <summary>
        /// computes sample weightings and texture coordinate offsets for one pass of a separable gaussian blur filter.
        /// </summary>
        void SetBlurOffsetParameter()
        {
            Vector2[] offsets = new Vector2[_sampleCount];

            // Add pairs of additional sample taps, positioned along a line in both directions from the center.
            for (var i = 0; i < _sampleCount / 2; i++)
            {
                // To get the maximum amount of blurring from a limited number of pixel shader samples, we take advantage of the bilinear filtering
                // hardware inside the texture fetch unit. If we position our texture coordinates exactly halfway between two texels, the filtering unit
                // will average them for us, giving two samples for the price of one. This allows us to step in units of two texels per sample, rather
                // than just one at a time. The 1.5 offset kicks things off by positioning us nicely in between two texels.
                var sampleOffset = i * 2 + 1.5f;

                var delta = _inverseResolution * sampleOffset;

                // Store texture coordinate offsets for the positive and negative taps.
                offsets[i * 2 + 1] = delta;
                offsets[i * 2 + 2] = -delta;
            }

            _blurOffsetsParam.SetValue(offsets);
        }

        /// <summary>
		/// calculates the sample weights and passes them along to the shader
		/// </summary>
		void CalculateSampleWeights()
        {
            // The first sample always has a zero offset.
            _sampleWeights[0] = ComputeGaussian(0);

            // Maintain a sum of all the weighting values.
            var totalWeights = _sampleWeights[0];

            // Add pairs of additional sample taps, positioned along a line in both directions from the center.
            for (var i = 0; i < _sampleCount / 2; i++)
            {
                // Store weights for the positive and negative taps.
                var weight = ComputeGaussian(i + 1);

                _sampleWeights[i * 2 + 1] = weight;
                _sampleWeights[i * 2 + 2] = weight;

                totalWeights += weight * 2;
            }

            // Normalize the list of sample weightings, so they will always sum to one.
            for (var i = 0; i < _sampleWeights.Length; i++)
                _sampleWeights[i] /= totalWeights;

            // Tell the effect about our new filter settings.
            _blurWeightsParam.SetValue(_sampleWeights);
        }

        /// <summary>
        /// Evaluates a single point on the gaussian falloff curve.
        /// Used for setting up the blur filter weightings.
        /// </summary>
        float ComputeGaussian(float n)
        {
            return (float)((1.0 / Math.Sqrt(2 * Math.PI * _blurAmount)) *
                            Math.Exp(-(n * n) / (2 * _blurAmount * _blurAmount)));
        }
        #endregion
    }
}
