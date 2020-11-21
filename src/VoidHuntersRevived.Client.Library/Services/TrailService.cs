using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.Collections;
using Guppy.Extensions.System;
using Guppy.Extensions.Utilities;
using Guppy.Lists;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Drivers.Entities.ShipParts.Thrusters;
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
        private ServiceList<Trail> _trails;

        private PrimitiveBatch _primitiveBatch;
        private FarseerCamera2D _camera;

        private ActionTimer _segmentTimer;

        private DebugService _debug;
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

            provider.Service(out _primitiveBatch);
            provider.Service(out _camera);
            provider.Service(out _trails);
            provider.Service(out _debug);

            _segmentTimer = new ActionTimer(150);
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

            _trails.ForEach(trail => trail.TryUpdate(gameTime));
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _primitiveBatch.Begin(_camera, BlendState.AlphaBlend);
            _trails.ForEach(t => t.TryDraw(gameTime));
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
