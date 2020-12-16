﻿using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.Microsoft.Xna.Framework;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

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
        public static Single MaxAge { get; set; } = 10f;

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

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);
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

            // Update the internal vertece data as required.
            this.PortVertex.Position = this.Position;
            this.PortVertex.SpreadDirection = this.Rotation - MathHelper.PiOver2;
            this.PortVertex.CreatedTimestamp =  (Single)gameTime.TotalGameTime.TotalSeconds;
            this.PortVertex.Color = this.Color;


            this.StarboardVertex.Position = this.Position;
            this.StarboardVertex.SpreadDirection = this.Rotation + MathHelper.PiOver2;
            this.StarboardVertex.CreatedTimestamp = (Single)gameTime.TotalGameTime.TotalSeconds;
            this.StarboardVertex.Color = this.Color;
        }
        #endregion
    }
}
