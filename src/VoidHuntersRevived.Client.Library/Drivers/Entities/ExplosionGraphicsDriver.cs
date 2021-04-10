using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.Microsoft.Xna.Framework;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Graphics.Effects;
using VoidHuntersRevived.Client.Library.Graphics.Vertices;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Extensions.System;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities
{
    internal sealed class ExplosionGraphicsDriver : Driver<Explosion>
    {
        #region Static Fields
        private static readonly Int32 ParticleCount = 32;
        private static readonly Random Noise = new Random();
        #endregion

        #region Private Fields
        private PrimitiveBatch<VertexExplosion, ExplosionEffect> _primitiveBatch;
        private VertexExplosion _center;
        private VertexExplosion[] _particles;
        private Random _random;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(Explosion driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            provider.Service(out _primitiveBatch);

            _particles = new VertexExplosion[ExplosionGraphicsDriver.ParticleCount];

            this.driven.OnDraw += this.Draw;
            this.driven.OnPreDraw += this.Setup;
        }

        protected override void Release(Explosion driven)
        {
            base.Release(driven);

            _primitiveBatch = null;

            this.driven.OnDraw -= this.Draw;
            this.driven.OnPreDraw -= this.Setup;
        }
        #endregion

        #region Frame Methods
        private void Setup(GameTime gameTime)
        {
            var currentTimestamp = (Single)gameTime.TotalGameTime.TotalSeconds;

            _center = new VertexExplosion()
            {
                Position = this.driven.Context.StartPosition,
                Velocity = this.driven.Context.StartVelocity,
                MaxAge = this.driven.Context.MaxAge,
                Color = this.driven.Context.Color,
                MaxRadius = 0,
                Direction = 0,
                CreatedTimestamp = currentTimestamp
            };

            for (Int32 i = 0; i < ExplosionGraphicsDriver.ParticleCount; i++)
            {
                _particles[i].Position = this.driven.Context.StartPosition;
                _particles[i].Velocity = this.driven.Context.StartVelocity;
                _particles[i].MaxAge = this.driven.Context.MaxAge;
                _particles[i].Color = this.driven.Context.Color;
                _particles[i].MaxRadius = this.driven.Context.MaxRadius * ExplosionGraphicsDriver.Noise.NextSingle(0.75f, 1.5f);
                _particles[i].Direction = (MathHelper.TwoPi / ExplosionGraphicsDriver.ParticleCount) * i;
                _particles[i].CreatedTimestamp = currentTimestamp;
            }

            this.driven.OnPreDraw -= this.Setup;
        }
        private void Draw(GameTime gameTime)
        {
            for (Int32 i=0; i< ExplosionGraphicsDriver.ParticleCount; i++)
            {
                _primitiveBatch.DrawTriangle(
                    _center, 
                    _particles[i], 
                    _particles[(i + 1) % ExplosionGraphicsDriver.ParticleCount]);
            }
        }
        #endregion
    }
}
