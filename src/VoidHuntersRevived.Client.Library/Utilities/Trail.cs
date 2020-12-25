using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.Collections;
using Guppy.Interfaces;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Library.Utilities;
using Guppy.Extensions.DependencyInjection;
using Guppy.Events.Delegates;
using VoidHuntersRevived.Client.Library.Services;
using System.Drawing;
using Guppy.Utilities.Primitives;
using Color = Microsoft.Xna.Framework.Color;
using Guppy.Extensions.System;
using VoidHuntersRevived.Library.Extensions.Microsoft.Xna;
using Guppy.Extensions.Utilities;
using Guppy.Extensions.Microsoft.Xna.Framework;
using VoidHuntersRevived.Client.Library.Effects;
using VoidHuntersRevived.Client.Library.Utilities.Vertices;
using Guppy.Enums;

namespace VoidHuntersRevived.Client.Library.Utilities
{
    /// <summary>
    /// Class containing a list of trail segments.
    /// Each segment represents a single point of 
    /// thrust the thruster passed.
    /// </summary>
    public sealed class Trail : Frameable
    {
        #region Static Properties
        /// <summary>
        /// The maximum segment age (in seconds)
        /// before it should be deleted.
        /// </summary>
        public static Double MaxSegmentAge = 10;

        public static Single MaxAlphaMultiplier = 0.2f;
        #endregion

        #region Private Fields
        private ServiceProvider _provider;
        private PrimitiveBatch<TrailVertex, TrailInterpolationEffect> _primitiveBatch;

        private Single _top, _right, _bottom, _left;
        private Queue<TrailSegment> _segments;
        private TrailSegment _youngestSegment;
        private Boolean _addedInitialSegment;
        private Thruster _thruster;
        private Synchronizer _synchronizer;
        #endregion

        #region Public Properties
        public IReadOnlyCollection<TrailSegment> Segments => _segments;
        public Thruster Thruster
        {
            get => _thruster;
            set => this.OnThrusterChanged.InvokeIf(value != _thruster, this, ref _thruster, value);
        }
        public RectangleF Bounds => new RectangleF(
            x: _left, 
            y: _top, 
            width: _right - _left, 
            height: _bottom - _top);
        #endregion

        #region Event Handlers
        public OnChangedEventDelegate<Trail, Thruster> OnThrusterChanged;
        #endregion

        #region Constructors
        internal Trail()
        {

        }
        #endregion

        #region Lifecycle Methods
        protected override void Create(ServiceProvider provider)
        {
            base.Create(provider);

            provider.Service(out _primitiveBatch);

            this.OnThrusterChanged += this.HandleThrusterChanged;
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _synchronizer);

            _provider = provider;
            _segments = new Queue<TrailSegment>();
            _addedInitialSegment = false;
        }

        protected override void Release()
        {
            base.Release();

            _youngestSegment = null;
            while (_segments.Any())
                _segments.Dequeue().TryRelease();

            this.Thruster = null;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_segments.Any())
            {
                // Refresh the youngest vertex...
                if (this.Thruster != default)
                    _youngestSegment.Setup(gameTime, this);

                // Clear the internal boundry data
                _top = Single.MaxValue;
                _right = 0;
                _bottom = 0;
                _left = Single.MaxValue;

                // Update each vertice & update bounds data.
                foreach(TrailSegment segment in _segments)
                {
                    _top = Math.Min(_top, segment.Position.Y);
                    _right = Math.Max(_right, segment.Position.X);
                    _bottom = Math.Max(_bottom, segment.Position.Y);
                    _left = Math.Min(_left, segment.Position.X);
                }


                if ((gameTime.TotalGameTime.TotalSeconds - _segments.First().CreatedTimestamp) > TrailSegment.MaxAge)
                    _segments.Dequeue().TryRelease();
            }
            else if (!_addedInitialSegment)
            { // No initial segment has been added...
                this.AddSegment(gameTime);
                this.AddSegment(gameTime);
                _addedInitialSegment = true;
            }
            else // Auto release once no segments exist...
                _synchronizer.Enqueue(gt => this.TryRelease());
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            foreach(TrailSegment segment in _segments.Skip(1))
            {
                _primitiveBatch.DrawTriangle(
                    v1: ref segment.PortVertex,
                    v2: ref segment.StarboardVertex,
                    v3: ref segment.OlderSibling.PortVertex);

                _primitiveBatch.DrawTriangle(
                    v1: ref segment.StarboardVertex,
                    v2: ref segment.OlderSibling.StarboardVertex,
                    v3: ref segment.OlderSibling.PortVertex);
            }
        }

        // protected override void DebugDraw(GameTime gameTime)
        // {
        //     base.Draw(gameTime);
        // 
        //     Trail.LastSegment = _segments.First();
        // 
        //     foreach (TrailSegment segment in _segments.Skip(1))
        //     {
        //         _primitiveBatch.DrawLine(segment.Color, Trail.LastSegment.Position, segment.Position);
        //         _primitiveBatch.DrawLine(segment.Color, segment.Position + segment.StarboardSpread, segment.Position + segment.PortSpread);
        //         Trail.LastSegment = segment;
        //     }
        // 
        //     _primitiveBatch.DrawRectangleF(Color.Red, this.Bounds);
        // }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Attempt to create a brand new segment 
        /// based on the current trail's 
        /// thruster's status.
        /// </summary>
        /// <param name="gameTime"></param>
        public void TryAddSegment(GameTime gameTime)
        {
            if(this.Thruster?.ImpulseModifier > 0.1f)
            { // Only add a segment if this trail is not orphan & applying thrust...
                this.AddSegment(gameTime);
            }
        }

        /// <summary>
        /// Add a segment, no checks done at all
        /// </summary>
        /// <param name="gameTime"></param>
        public void AddSegment(GameTime gameTime)
        {
            _segments.Enqueue(_youngestSegment = _provider.GetService<TrailSegment>((segment, p, c) =>
            {
                segment.Setup(gameTime, this);
                segment.OlderSibling = _youngestSegment;
            }));
        }
        #endregion

        #region Events
        private void HandleThrusterReleasing(IService sender)
            => this.Thruster = null;

        private void HandleThrusterChanged(Trail sender, Thruster old, Thruster value)
        {
            if (old != default)
            { // Only proceed if the trail is not already an orphan...
                // Remove all event handlers...
                old.OnStatus[ServiceStatus.Releasing] -= this.HandleThrusterReleasing;
            }

            if(value != default)
            {
                // Add all event handlers...
                value.OnStatus[ServiceStatus.Releasing] += this.HandleThrusterReleasing;
            }
        }
        #endregion
    }
}
