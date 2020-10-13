using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace VoidHuntersRevived.Library.Entities.Ammunitions
{
    public class Bullet : Ammunition
    {
        #region Private Fields
        private PrimitiveBatch _primitiveBatch;
        #endregion

        #region Public Properties
        public Vector2 Position { get; internal set; }
        public Vector2 Velocity { get; internal set; }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _primitiveBatch);
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _primitiveBatch.DrawLine(Color.Red, this.Position, this.Position + (this.Velocity * 0.05f));
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Position += this.Velocity * (Single)gameTime.ElapsedGameTime.TotalSeconds;
        }
        #endregion
    }
}
