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
using System.IO;
using System.Text;
using VoidHuntersRevived.Client.Library.Drivers.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Client.Library.Effects;
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

        private PrimitiveBatch<VertexTrailSegment, TrailInterpolationEffect> _primitiveBatch;
        private FarseerCamera2D _camera;
        private DebugService _debug;
        private GameWindow _window;
        private ContentService _content;
        private GraphicsDevice _graphics;
        private SpriteBatch _spriteBatch;

        private ActionTimer _segmentTimer;
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

            _segmentTimer = new ActionTimer(32);

            _debug.Lines += this.HandleDebugLines;
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

            _primitiveBatch.Begin(_camera, BlendState.NonPremultiplied);
            _trails.TryDraw(gameTime);
            _primitiveBatch.End();
        }
        #endregion

        #region Helper Methods
        internal Trail BuildTrail(Thruster thruster)
            => _trails.Create<Trail>((trail, p, c) => trail.Thruster = thruster);
        #endregion

        #region Event Handlers
        private string HandleDebugLines(GameTime gameTime)
            => $"Trails: {_trails.Count.ToString("#,###,##0")}";
        #endregion
    }
}
