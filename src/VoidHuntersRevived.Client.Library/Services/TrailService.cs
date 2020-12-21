using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.Collections;
using Guppy.Extensions.System;
using Guppy.Extensions.Utilities;
using Guppy.Lists;
using Guppy.Services;
using Guppy.Utilities;
using Guppy.Utilities.Cameras;
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
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Client.Library.Services
{
    public sealed class TrailService : Frameable
    {
        #region Private Fields
        private ServiceProvider _provider;
        private PrimitiveBatch<TrailVertex, TrailInterpolationEffect> _primitiveBatch;
        private Camera2D _camera;
        private ActionTimer _segmentTimer;
        private List<Trail> _trails;
        #endregion


        #region Frame Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            _provider = provider;
            provider.Service(out _primitiveBatch);
            provider.Service(out _camera);

            _segmentTimer = new ActionTimer(250);
            _trails = new List<Trail>();

            _primitiveBatch.Effect.MaxAge = 10f;
            _primitiveBatch.Effect.SpreadSpeed = 3f;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _trails.ForEach(trail =>
            {
                trail.TryUpdate(gameTime);
            });

            _segmentTimer.Update(gameTime, gt =>
            {
                _trails.ForEach(trail =>
                {
                    trail.TryAddSegment(gameTime);
                });
            });
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _primitiveBatch.Effect.CurrentTimestamp = (Single)gameTime.TotalGameTime.TotalSeconds;
            _primitiveBatch.Begin(_camera, BlendState.NonPremultiplied);
            _trails.ForEach(trail =>
            {
                trail.TryDraw(gameTime);
            });
            _primitiveBatch.End();
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Build a new trail bound to the recieved <paramref name="thruster"/>
        /// and return its instance.
        /// </summary>
        /// <param name="thruster"></param>
        /// <returns></returns>

        public Trail BuildTrail(Thruster thruster)
            => _provider.GetService<Trail>((trail, p, c) =>
            {
                trail.Thruster = thruster;
                _trails.Add(trail);
            });
        #endregion
    }
}
