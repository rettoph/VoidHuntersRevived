using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.Collections;
using Guppy.Extensions.System;
using Guppy.Extensions.Utilities;
using Guppy.Lists;
using Guppy.Services;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Drivers.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Client.Library.Effects.Bloom;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Client.Library.Services
{
    public sealed class TrailService : Frameable
    {
        #region Private Fields
        private FrameableList<Trail> _trails;

        private PrimitiveBatch _primitiveBatch;
        private FarseerCamera2D _camera;
        private DebugService _debug;
        private GameWindow _window;
        private ContentService _content;
        private GraphicsDevice _graphics;
        private SpriteBatch _spriteBatch;

        private Effect _blur;

        private ActionTimer _segmentTimer;
        private RenderTarget2D _target;
        #endregion

        #region Constructors
        internal TrailService()
        {

        }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _spriteBatch);
            provider.Service(out _primitiveBatch);
            provider.Service(out _camera);
            provider.Service(out _trails);
            provider.Service(out _debug);
            provider.Service(out _window);
            provider.Service(out _content);
            provider.Service(out _graphics);

            _blur = _content.Get<Effect>("effect:blur");

            _segmentTimer = new ActionTimer(250);
            _debug.Lines += this.HandleDebugLines;
            _window.ClientSizeChanged += this.HandleClientSizeChanged;

            this.CleanTarget();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _segmentTimer.Update(gameTime, gt =>
            {
                _trails.ForEach(trail => trail.TryAddSegment());
            });

            _trails.TryUpdate(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            var targets = _graphics.GetRenderTargets();
            _graphics.SetRenderTarget(_target);
            _graphics.Clear(Color.Transparent);
            
            // Draw the trails on our render target...
            _primitiveBatch.Begin(_camera, BlendState.AlphaBlend);
            _trails.TryDraw(gameTime);
            _primitiveBatch.End();

            _graphics.SetRenderTargets(targets);

            _spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, blendState: BlendState.Additive);

            foreach (EffectPass pass in _blur.CurrentTechnique.Passes)
            {
                pass.Apply();

                _spriteBatch.Draw(
                    texture: _target,
                    position: Vector2.Zero,
                    sourceRectangle: _target.Bounds,
                    color: Color.White,
                    rotation: 0f,
                    origin: Vector2.Zero,
                    scale: 1f,
                    effects: SpriteEffects.None,
                    layerDepth: 0);
            }

            _spriteBatch.End();
        }
        #endregion

        #region Helper Methods
        internal Trail BuildTrail(Thruster thruster)
            => _trails.Create<Trail>((trail, p, c) => trail.Thruster = thruster);

        private void CleanTarget()
        {
            _target?.Dispose();
            _target = new RenderTarget2D(_graphics, _graphics.Viewport.Width, _graphics.Viewport.Height);

            _blur.Parameters["InverseWidth"].SetValue(1f / _graphics.Viewport.Width);
            _blur.Parameters["InverseHeight"].SetValue(1f / _graphics.Viewport.Height);
            _blur.Parameters["Strength"].SetValue(2f);
        }
        #endregion

        #region Event Handlers
        private string HandleDebugLines(GameTime gameTime)
            => $"Trails: {_trails.Count.ToString("#,###,##0")}";

        private void HandleClientSizeChanged(object sender, EventArgs e)
            => this.CleanTarget();
        #endregion
    }
}
