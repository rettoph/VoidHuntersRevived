using System.Diagnostics.CodeAnalysis;
using Guppy.Core.Common.Attributes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;
using VoidHuntersRevived.Game.Core.Graphics.Effects;

namespace VoidHuntersRevived.Game.Client.Systems
{
    internal class ShaderAntiAliasingSystem : StrategySystem, IDisposable
    {
        private readonly GraphicsDevice _graphics;
        private readonly SpriteBatch _spriteBatch;
        private readonly GameWindow _window;
        private readonly ShaderAntiAliasingEffect _effect_aa;
        private RenderTarget2D _target_aa;
        private RenderTargetBinding[] _target_bindings;

        public ShaderAntiAliasingSystem(GraphicsDevice graphics, SpriteBatch spriteBatch, GameWindow window, ShaderAntiAliasingEffect effect_aa)
        {
            this._graphics = graphics;
            this._spriteBatch = spriteBatch;
            this._window = window;
            this._effect_aa = effect_aa;

            this._target_aa = this.BuildRenderTarget();
            this._target_bindings = [];

            this._window.ClientSizeChanged += this.HandleClientSizeChanged;
        }

        public void Dispose()
        {
            this._target_aa?.Dispose();
        }

        private RenderTarget2D BuildRenderTarget()
        {
            this._effect_aa.Pixel = new Vector2(1f / this._graphics.Viewport.Width, 1f / this._graphics.Viewport.Height);
            return new RenderTarget2D(this._graphics, this._graphics.Viewport.Width, this._graphics.Viewport.Height, true, SurfaceFormat.Color, DepthFormat.None, this._graphics.PresentationParameters.MultiSampleCount, RenderTargetUsage.PreserveContents);
        }

        [SequenceGroup<OnDrawSequenceGroupEnum>(OnDrawSequenceGroupEnum.PreDraw)]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Duck typing")]
        public void PreDraw(GameTime gameTime)
        {
            this._target_bindings = this._graphics.GetRenderTargets();

            this._graphics.SetRenderTarget(this._target_aa);
            this._graphics.Clear(Color.Transparent);
        }

        [SequenceGroup<OnDrawSequenceGroupEnum>(OnDrawSequenceGroupEnum.PostDraw)]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Duck typing")]
        public void PostDraw(GameTime gameTime)
        {
            this._graphics.SetRenderTargets(this._target_bindings);

            this._spriteBatch.Begin(effect: this._effect_aa);
            this._spriteBatch.Draw(this._target_aa, Vector2.Zero, Color.White);
            this._spriteBatch.End();
        }

        private void HandleClientSizeChanged(object? sender, EventArgs e)
        {
            this._target_aa?.Dispose();
            this._target_aa = this.BuildRenderTarget();
        }
    }
}