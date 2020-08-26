using Guppy;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;
using Guppy.Extensions.DependencyInjection;

namespace VoidHuntersRevived.Client.Library.Entities
{
    public sealed class TrailManager : Entity
    {
        #region Private Fields
        private ServiceProvider _provider;
        private Dictionary<Thruster, Trail> _trails;
        private Queue<Trail> _dirtyTrails;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _provider = provider;

            _trails = new Dictionary<Thruster, Trail>();
            _dirtyTrails = new Queue<Trail>();

            this.LayerGroup = 10;
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            while (_dirtyTrails.Any())
                _dirtyTrails.Dequeue().Draw(gameTime);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Setup a new trail manager...
        /// </summary>
        /// <param name="thruster"></param>
        internal Trail SetupTrail(Thruster thruster)
            => _trails[thruster] = _provider.GetService<Trail>((t, p, c) =>
            {
                t.thruster = thruster;

                t.OnDisposed += this.HandleTrailDisposed;
                t.OnDirty += this.HandleTrailDirty;
            }); 

        private void RemoveTrail(Trail trail)
        {
            trail.OnDisposed -= this.HandleTrailDisposed;
            trail.OnDirty -= this.HandleTrailDirty;

            _trails.Remove(trail.thruster);
        }
        #endregion

        #region Event Handlers
        private void HandleTrailDisposed(IService sender)
            => this.RemoveTrail(sender as Trail);

        private void HandleTrailDirty(Trail sender)
            => _dirtyTrails.Enqueue(sender);
        #endregion
    }
}
