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
        #endregion

        #region Helper Methods
        public ExplosionParticles(GameTime gameTime, Explosion explosion)
        {
            var now = (Single)gameTime.TotalGameTime.TotalSeconds;


            this.Center.Color = explosion.Color;
            this.Center.CreatedTimestamp = now;
            this.Center.Position = explosion.Position;
            this.Center.Velocity = explosion.Velocity;

            this.Vertices = new ExplosionVertex[ExplosionParticles.ParticleCount];
            var step = MathHelper.TwoPi / ExplosionParticles.ParticleCount;

            for (Int32 i=0; i < ExplosionParticles.ParticleCount; i++)
            {
                this.Vertices[i].Color = explosion.Color;
                this.Vertices[i].CreatedTimestamp = now;
                this.Vertices[i].Position = explosion.Position;
                this.Vertices[i].Velocity = explosion.Velocity + Vector2.UnitX.Rotate(i * step);
            }
        }
        #endregion
    }
}
