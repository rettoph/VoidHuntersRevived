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

namespace VoidHuntersRevived.Client.Library.Utilities
{
    /// <summary>
    /// Class containing a list of trail segments.
    /// Each segment represents a single point of 
    /// thrust the thruster passed.
    /// </summary>
    internal sealed class Trail : Frameable
    {
        #region Static Properties
        /// <summary>
        /// The maximum segment age (in seconds)
        /// before it should be deleted.
        /// </summary>
        public static Double MaxSegmentAge = 10;

        private static Single MaxColorMultiplier = 2f;
        #endregion

        #region Private Fields
        private ServiceProvider _provider;
        private PrimitiveBatch<VertexTrailSegment, TrailInterpolationEffect> _primitiveBatch;

        private Single _top, _right, _bottom, _left;
        private Queue<TrailSegment> _segments;
        private TrailSegment _youngestSegment;
        #endregion

        #region Public Properties
        public IReadOnlyCollection<TrailSegment> Segments => _segments;
        public Thruster Thruster { get; internal set; }
        public RectangleF Bounds => new RectangleF(
            x: _left, 
            y: _top, 
            width: _right - _left, 
            height: _bottom - _top);
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
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            _provider = provider;

            _segments = new Queue<TrailSegment>();
            this.TryAddSegment(); // Add a segment on start...

            this.Thruster.OnReleased += this.HandleThrusterReleased;
        }

        protected override void Release()
        {
            base.Release();

            this.TryDisown();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_segments.Any())
            {
                _top = Single.MaxValue;
                _right = 0;
                _bottom = 0;
                _left = Single.MaxValue;

                _segments.ForEach(segment =>
                {
                    segment.TryUpdate(gameTime);

                    _top    = Math.Min(_top   , segment.Position.Y);
                    _right  = Math.Max(_right , segment.Position.X);
                    _bottom = Math.Max(_bottom, segment.Position.Y);
                    _left   = Math.Min(_left  , segment.Position.X);
                });

                if (_segments.First().Age >= Trail.MaxSegmentAge)
                {
                    _segments.Dequeue().TryRelease();

                    // Auto release once all segments have expired.
                    if(!_segments.Any())
                        this.TryRelease();
                }
            }
            else // Auto release once no segments exist...
                this.TryRelease();
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            foreach(TrailSegment segment in _segments.Skip(1))
            {
                segment.TryDraw(gameTime);
            }

            if (this.Thruster?.Chain.Ship != default)
            {
                var color = Color.Lerp(Color.Transparent, this.Thruster.Color, this.Thruster.ImpulseModifier * Trail.MaxColorMultiplier);
                color = this.Thruster.Color;
                // _primitiveBatch.DrawTriangle(Color.Transparent, _youngestSegment.Port, color, this.Thruster.Position, _youngestSegment.Color, _youngestSegment.Position);
                // _primitiveBatch.DrawTriangle(Color.Transparent, _youngestSegment.Starboard, _youngestSegment.Color, _youngestSegment.Position, color, this.Thruster.Position);
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
        public void TryAddSegment()
        {
            if(this.Thruster?.ImpulseModifier > Thruster.ImpulseModifierEpsilon)
            { // Only add a segment if this trail is not orphan & applying thrust...
                _segments.Enqueue(_youngestSegment = _provider.GetService<TrailSegment>((segment, p, c) =>
                {
                    segment.Position = this.Thruster.Position;
                    segment.Velocity = (this.Thruster.Root.LinearVelocity + Thruster.FullImpulse.RotateTo(this.Thruster.Rotation + MathHelper.Pi)) / 1.75f;
                    segment.Rotation = this.Thruster.Rotation;
                    segment.BaseColor = Color.Lerp(Color.Transparent, this.Thruster.Color, this.Thruster.ImpulseModifier * Trail.MaxColorMultiplier);
                    segment.SpreadModifier = this.Thruster.ImpulseModifier;
                    segment.OlderSibling = _youngestSegment;
                }));
            }
        }

        /// <summary>
        /// Used when the owning thruster disowns
        /// the current trail.
        /// </summary>
        internal void TryDisown()
        {
            if (this.Thruster != default)
            { // Only proceed if the trail is not already an orphan...
                // Remove all event handlers...
                this.Thruster.OnReleased -= this.HandleThrusterReleased;

                // Unbind the internal thruster, becoming an orphan
                this.Thruster = null;
            }
        }
        #endregion

        #region Events
        private void HandleThrusterReleased(IService sender)
            => this.TryDisown();
        #endregion
    }
}
