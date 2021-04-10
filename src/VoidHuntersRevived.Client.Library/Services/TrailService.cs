using Guppy;
using Guppy.DependencyInjection;
using Guppy.Lists;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Windows.Library.Graphics.Vertices;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;

namespace VoidHuntersRevived.Windows.Library.Services
{
    /// <summary>
    /// Simple service used to manage TrailSegment's.
    /// </summary>
    public class TrailService : Frameable
    {
        #region Private Fields
        private EntityList _entities;
        private ServiceList<Trail> _trails;
        private ActionTimer _actions;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _entities);
            provider.Service(out _trails);

            _actions = new ActionTimer(100);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _actions.Update(gameTime, gt =>
            {
                foreach (Trail trail in _trails)
                    trail.TryAddSegment(gameTime);
            });
        }
        #endregion

        #region API Methods
        public Trail Create(Thruster thruster)
            => _entities.Create<Trail>((t, p, c) =>
            {
                t.Thruster = thruster;
                _trails.TryAdd(t);
            });
        #endregion
    }
}
