using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.Microsoft.Xna.Framework;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities.Vertices;

namespace VoidHuntersRevived.Client.Library.Utilities
{
    /// <summary>
    /// Simple class that contains
    /// 2 <see cref="TrailVertex"/> instances,
    /// once for the Port and Starboard side of the relative
    /// trail. A <see cref="TrailSegment"/> is rendered when another
    /// <see cref="TrailSegment"/> is given, allowing for the ability
    /// to create a full quad.
    /// 
    /// If the 
    /// </summary>
    public class TrailSegment : Service
    {
        #region Public Properties
        public static Single MaxAge { get; set; } = 5f;

        /// <summary>
        /// The current segment position, based on the
        /// <see cref="Trail.Thruster.Position"/>
        /// value on input.
        /// </summary>
        public Vector2 Position { get; private set; }

        /// <summary>
        /// The current segment rotation, based on the
        /// <see cref="Trail.Thruster.Rotation"/>
        /// value on input.
        /// </summary>
        public Single Rotation { get; private set; }

        public Color Color { get; private set; }

        public Vector2 ReverseImpulse { get; private set; }
        public Single CreatedTimestamp { get; private set; }

        /// <summary>
        /// The current segments older sibling, if any.
        /// The siblings Port and Starboard vertices are
        /// used to generate a quad to render the current trail 
        /// segment.
        /// </summary>
        public TrailSegment OlderSibling { get; internal set; }
        #endregion

        #region Public Fields
        public TrailVertex PortVertex;
        public TrailVertex StarboardVertex;
        #endregion

        #region Constructor
        internal TrailSegment()
        {

        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);
        }

        protected override void Release()
        {
            base.Release();

            this.OlderSibling = null;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Update the internal segment data based on the current state
        /// of the recieved <see cref="Trail"/> & its relevant 
        /// <see cref="Trail.Thruster"/> data.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="trail"></param>
        internal void Setup(GameTime gameTime, Trail trail)
        {
            this.Position = trail.Thruster.Position;
            this.Rotation = trail.Thruster.Rotation;
            this.Color = new Color(trail.Thruster.Color, trail.Thruster.ImpulseModifier * Trail.MaxAlphaMultiplier);
            this.ReverseImpulse = trail.Thruster.Impulse.RotateTo(trail.Thruster.Rotation + MathHelper.Pi);
            this.CreatedTimestamp = (Single)gameTime.TotalGameTime.TotalSeconds;

            // Update the internal vertece data as required.
            this.PortVertex.Position = this.Position;
            this.PortVertex.SpreadDirection = this.Rotation - MathHelper.PiOver2;
            this.PortVertex.CreatedTimestamp = this.CreatedTimestamp;
            this.PortVertex.Color = this.Color;
            this.PortVertex.ReverseImpulse = this.ReverseImpulse;


            this.StarboardVertex.Position = this.Position;
            this.StarboardVertex.SpreadDirection = this.Rotation + MathHelper.PiOver2;
            this.StarboardVertex.CreatedTimestamp = this.CreatedTimestamp;
            this.StarboardVertex.Color = this.Color;
            this.StarboardVertex.ReverseImpulse = this.ReverseImpulse;
        }
        #endregion
    }
}
