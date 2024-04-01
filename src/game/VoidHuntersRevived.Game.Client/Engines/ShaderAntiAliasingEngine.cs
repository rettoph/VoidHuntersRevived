using Guppy.Attributes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Game.Client.Graphics.Effects;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [SimulationFilter(SimulationType.Predictive)]
    internal class ShaderAntiAliasingEngine : BasicEngine, IStepEngine<FrameStart>, IStepEngine<FrameEnd>, IDisposable
    {
        private readonly GraphicsDevice _graphics;
        private readonly SpriteBatch _spriteBatch;
        private readonly GameWindow _window;
        private readonly ShaderAntiAliasingEffect _effect_aa;
        private RenderTarget2D _target_aa;
        private RenderTargetBinding[] _target_bindings;

        public string name => nameof(ShaderAntiAliasingEngine);

        public ShaderAntiAliasingEngine(GraphicsDevice graphics, SpriteBatch spriteBatch, GameWindow window, ShaderAntiAliasingEffect effect_aa, ContentManager content)
        {
            _graphics = graphics;
            _spriteBatch = spriteBatch;
            _window = window;
            _effect_aa = effect_aa;

            _target_aa = this.BuildRenderTarget();
            _target_bindings = Array.Empty<RenderTargetBinding>();

            _window.ClientSizeChanged += this.HandleClientSizeChanged;
        }

        public void Dispose()
        {
            _target_aa?.Dispose();
        }

        private RenderTarget2D BuildRenderTarget()
        {
            return new RenderTarget2D(_graphics, _graphics.Viewport.Width, _graphics.Viewport.Height, true, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.PreserveContents);
        }


        public void Step(in FrameStart param)
        {
            _target_bindings = _graphics.GetRenderTargets();

            _graphics.SetRenderTarget(_target_aa);
            _graphics.Clear(Color.Transparent);
        }

        public void Step(in FrameEnd param)
        {
            _graphics.SetRenderTargets(_target_bindings);

            _spriteBatch.Begin();
            _spriteBatch.Draw(_target_aa, Vector2.Zero, Color.White);
            _spriteBatch.End();
        }

        private void HandleClientSizeChanged(object? sender, EventArgs e)
        {
            _target_aa?.Dispose();
            _target_aa = this.BuildRenderTarget();
        }
    }
}
