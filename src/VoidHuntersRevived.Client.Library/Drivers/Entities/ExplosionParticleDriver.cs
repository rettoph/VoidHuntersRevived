using Guppy;
using Guppy.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Services;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities
{
    internal sealed class ExplosionParticleDriver : Driver<Explosion>
    {
        #region Private Fields 
        private ExplosionRenderService _particles;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(Explosion driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            provider.Service(out _particles);

            this.driven.OnDraw += this.Draw;
        }

        protected override void Release(Explosion driven)
        {
            base.Release(driven);

            this.driven.OnDraw -= this.Draw;
        }
        #endregion

        #region Frame Methods
        private void Draw(GameTime gameTime)
        {
            _particles.Configure(gameTime, this.driven);

            this.driven.OnDraw -= this.Draw;
        }
        #endregion
    }
}
