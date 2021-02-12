using Guppy.Extensions.Microsoft.Xna.Framework;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities.Vertices;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Client.Library.Utilities
{
    public class ExplosionParticles
    {
        #region Static Properties
        /// <summary>
        /// The amount of particles to include within each explosion.
        /// </summary>
        public static Int32 ParticleCount { get; set; } = 30;

        /// <summary>
        /// Used for calculating particle variances.
        /// </summary>
        private static Random rand = new Random();
        #endregion

        #region Public Fields
        /// <summary>
        /// The center point of the explosion.
        /// </summary>
        public ExplosionVertex Center;

        /// <summary>
        /// The outer bounds of the explosion.
        /// </summary>
        public ExplosionVertex[] Vertices;

        /// <summary>
        /// The created timestamp to be passed
        /// into all vertices.
        /// </summary>
        public Single CreatedTimestamp;

        /// <summary>
        /// The timestamp at which the described particles
        /// should expire
        /// </summary>
        public Double ExpireTimestamp;

        /// <summary>
        /// The maximum age of the descirbed particles
        ///  in seconds.
        /// </summary>
        public Single MaxAge = 3;
        #endregion

        #region Helper Methods
        public ExplosionParticles(WorldEntity.ExplosionData explosion, GameTime gameTime)
        {
            this.CreatedTimestamp = (Single)gameTime.TotalGameTime.TotalSeconds;
            this.ExpireTimestamp = gameTime.TotalGameTime.TotalSeconds + this.MaxAge;

            this.Center.Position = explosion.Position;
            this.Center.Color = explosion.Color;
            this.Center.MaxRadius = 0;
            this.Center.CreatedTimestamp = this.CreatedTimestamp;
            this.Center.Direction = 0;
            this.Center.Alpha = 1f;
            this.Center.MaxAge = this.MaxAge;

            this.Vertices = new ExplosionVertex[ExplosionParticles.ParticleCount];
            var step = MathHelper.TwoPi / ExplosionParticles.ParticleCount;

            for (Int32 i=0; i < ExplosionParticles.ParticleCount; i++)
            {
                this.Vertices[i].Position = explosion.Position;
                this.Vertices[i].Color = explosion.Color;
                this.Vertices[i].MaxRadius = explosion.Radius * (1 + (Single)rand.NextDouble() * 0.5f);
                this.Vertices[i].CreatedTimestamp = this.CreatedTimestamp;
                this.Vertices[i].Direction = i * step;
                this.Vertices[i].Alpha = 0f;
                this.Vertices[i].MaxAge = this.MaxAge;
            }
        }
        #endregion
    }
}
