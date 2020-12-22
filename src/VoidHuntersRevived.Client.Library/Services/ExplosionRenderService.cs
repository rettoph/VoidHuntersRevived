using Guppy;
using Guppy.DependencyInjection;
using Guppy.Utilities;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Effects;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Client.Library.Utilities.Vertices;
using VoidHuntersRevived.Library.Entities;
using Guppy.Extensions.Collections;
using Guppy.Extensions.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace VoidHuntersRevived.Client.Library.Services
{
    /// <summary>
    /// Simple service used to render explosion
    /// particles.
    /// </summary>
    internal sealed class ExplosionRenderService : Frameable
    {
        #region Private Fields
        private PrimitiveBatch<ExplosionVertex, ExplosionEffect> _primitiveBatch;
        private Camera2D _camera;
        private List<ExplosionParticles> _particles;
        private Queue<ExplosionParticles> _toRemove;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _primitiveBatch);
            provider.Service(out _camera);

            _particles = new List<ExplosionParticles>();
            _toRemove = new Queue<ExplosionParticles>();
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Configure the particles for the recieved explosion.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="explosion"></param>
        public void Configure(GameTime gameTime, Explosion explosion)
            => _particles.Add(new ExplosionParticles(gameTime, explosion));
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);


            _particles.ForEach((p) =>
            {
                if (gameTime.TotalGameTime.TotalSeconds > p.ExpireTimestamp)
                    _toRemove.Enqueue(p);
            });

            while(_toRemove.Any())
                _particles.Remove(_toRemove.Dequeue());
        }


        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _primitiveBatch.Effect.CurrentTimestamp = (Single)gameTime.TotalGameTime.TotalSeconds;

            _primitiveBatch.Begin(_camera, BlendState.NonPremultiplied);

            _particles.ForEach((p) =>
            {
                Int32 i = 0;
                p.Vertices.ForEach((v) =>
                {
                    _primitiveBatch.DrawTriangle(v, p.Vertices[++i % p.Vertices.Length], p.Center);
                });
                
            });

            _primitiveBatch.End();
        }
        #endregion
    }
}
