using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Library.Contexts
{
    /// <summary>
    /// Defines context for the <see cref="Explosion"/>
    /// entity.
    /// </summary>
    public class ExplosionContext
    {
        /// <summary>
        /// The world position of the current explosion.
        /// </summary>
        public Vector2 StartPosition { get; set; }

        /// <summary>
        /// The velocity at which the explosion should travel
        /// </summary>
        public Vector2 StartVelocity { get; set; }

        /// <summary>
        /// The color of the current explosion.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// The maximum size of the explosion.
        /// </summary>
        public Single MaxRadius { get; set; } = 10f;

        /// <summary>
        /// The maximum force applied by the explosion per second.
        /// This decays based on explosion age and object distance from
        /// explosion.
        /// </summary>
        public Single MaxForcePerSecond { get; set; } = 5f;

        /// <summary>
        /// The maximum damage applied by the explosion per second.
        /// This decays based on explosion age and object distance from
        /// explosion.
        /// </summary>
        public Single MaxDamagePerSecond { get; set; } = 50f;

        /// <summary>
        /// The maximum age allowed (in seconds) before the explosion fully dissipates.
        /// </summary>
        public Single MaxAge { get; set; } = 2f;
    }
}
