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
        public static Int32 ParticleCount { get; set; } = 15;
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
        /// The timestamp at which the described particles
        /// should expire
        /// </summary>
        public Double ExpireTimestamp;

        /// <summary>
        /// The maximum age of the descirbed particles.
        /// </summary>
        public Double MaxAge;
        #endregion

        #region Helper Methods
        public ExplosionParticles(GameTime gameTime, Explosion explosion)
        {
            var now = (Single)gameTime.TotalGameTime.TotalSeconds - explosion.Age;
            var magnitude = (Single)Math.Sqrt(explosion.Velocity.Length());

            this.Center.Color = explosion.Color;
            this.Center.CreatedTimestamp = now;
            this.Center.Position = explosion.Position;
            this.Center.Direction = 0;
            this.Center.Magnitude = magnitude;
            this.Center.Alpha = 1f;
            this.Center.MaxAge = explosion.MaxAge;

            this.Vertices = new ExplosionVertex[ExplosionParticles.ParticleCount];
            var step = MathHelper.TwoPi / ExplosionParticles.ParticleCount;

            for (Int32 i=0; i < ExplosionParticles.ParticleCount; i++)
            {
                this.Vertices[i].Color = explosion.Color;
                this.Vertices[i].CreatedTimestamp = now;
                this.Vertices[i].Position = explosion.Position;
                this.Vertices[i].Direction = i * step;
                this.Vertices[i].Magnitude = (magnitude * 1.2f) + (explosion.Force * 0.1f);
                this.Vertices[i].Alpha = 0f;
                this.Vertices[i].MaxAge = explosion.MaxAge;
            }

            this.ExpireTimestamp = gameTime.TotalGameTime.TotalSeconds - explosion.Age + explosion.MaxAge;
        }
        #endregion
    }
}
