using Guppy;
using Guppy.EntityComponent;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Utilities;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Client.Library.Graphics.Effects;
using VoidHuntersRevived.Client.Library.Graphics.Utilities;
using VoidHuntersRevived.Client.Library.Graphics.Vertices;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;

namespace VoidHuntersRevived.Client.Library.Graphics.Services
{
    public sealed class TrailService : Frameable
    {
        #region Private Fields
        private List<Trail> _trails;
        private Queue<Trail> _trailsToActivate;
        private Queue<Trail> _trailsToRemove;

        private ActionTimer _segmentTimer;

        private PrimitiveBatch<VertexTrail, TrailEffect> _primitiveBatch;
        private Camera2D _camera;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _primitiveBatch);
            provider.Service(out _camera);

            _trails = new List<Trail>();
            _trailsToActivate = new Queue<Trail>();
            _trailsToRemove = new Queue<Trail>();

            _segmentTimer = new ActionTimer(64);
        }
        #endregion


        #region Public Properties
        public Trail Create(Thruster thruster)
        {
            var trail = new Trail(thruster);
            _trailsToActivate.Enqueue(trail);

            return trail;
        }
        #endregion

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        
            if(_trails.Count == 0)
            {
                return;
            }

            _primitiveBatch.Effect.CurrentTimestamp = (Single)gameTime.TotalGameTime.TotalSeconds;
            _primitiveBatch.Begin(_camera, BlendState.NonPremultiplied);

            foreach (Trail trail in _trails)
            {
                trail.Draw(gameTime, _primitiveBatch);
            }
            _primitiveBatch.End();

        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Ensure all new trails get activated...
            while(_trailsToActivate.Count > 0)
            {
                this.ActivateTrail(gameTime, _trailsToActivate.Dequeue());
            }

            // Ensure all old trails get removed...
            while (_trailsToRemove.Count > 0)
            {
                this.RemoveTrail(_trailsToRemove.Dequeue());
            }

            _segmentTimer.Update(gameTime, this.UpdateTrails);
        }

        private void UpdateTrails(GameTime gameTime)
        {
            foreach(Trail trail in _trails)
            {
                trail.Update(gameTime);
            }
        }

        private void ActivateTrail(GameTime gameTime, Trail trail)
        {
            trail.OnDisposed += this.HandleTrailDisposed;

            trail.Activate(gameTime);
            _trails.Add(trail);
        }

        private void RemoveTrail(Trail trail)
        {
            _trails.Remove(trail);

            trail.OnDisposed -= this.HandleTrailDisposed;
        }

        private void HandleTrailDisposed(Trail trail)
        {
            _trailsToRemove.Enqueue(trail);
        }
    }
}
