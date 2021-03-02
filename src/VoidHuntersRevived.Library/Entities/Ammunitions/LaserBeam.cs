using Guppy.DependencyInjection;
using Guppy.Extensions.Utilities;
using Guppy.Extensions.Microsoft.Xna.Framework;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts.Weapons;
using VoidHuntersRevived.Library.Entities.ShipParts;
using Guppy.Interfaces;
using Guppy.Enums;

namespace VoidHuntersRevived.Library.Entities.Ammunitions
{
    public class LaserBeam : Ammunition
    {
        #region Private Fields
        private PrimitiveBatch<VertexPositionColor> _primitiveBatch;

        private Vector2 _start;
        private Vector2 _end;
        private Int32 _count;
        #endregion

        #region Public Properties
        public Laser Laser { get; set; }

        public Single DamagePerSecond { get; set; }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _primitiveBatch);

            this.OnCollision += this.HandleCollision;
            this.Laser.OnStatus[ServiceStatus.Releasing] += this.HandleLaserReleasing;
        }

        protected override void Release()
        {
            base.Release();

            this.OnCollision -= this.HandleCollision;
            this.Laser.OnStatus[ServiceStatus.Releasing] -= this.HandleLaserReleasing;

            this.Laser = null;
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _primitiveBatch.DrawLine(
                this.Laser.Chain.Ship?.Color ?? Color.Transparent, 
                _start, 
                _end);
        }

        protected override void UpdateCollisions(GameTime gameTime)
        {
            _start = this.Laser.Position;
            _end = this.Laser.Position + 50f.ToVector2().RotateTo(this.Laser.Rotation + MathHelper.Pi);
            _count = 0;
            this.CheckCollisions(
                start: _start,
                end: _end,
                gameTime: gameTime);
        }
        #endregion

        /// <inheritdoc />
        #region Ammunition Implementation
        public override float GetShieldEnergyCost(GameTime gameTime)
        {
            return this.ShieldEnergyCost * (Single)gameTime.ElapsedGameTime.TotalSeconds;
        }
        #endregion

        #region Event Handlers
        private void HandleCollision(Ammunition sender, CollisionDataResult collision)
        {
            _count++;
            _end = collision.Data.P1;
        }

        private void HandleLaserReleasing(IService sender)
        {
            this.TryRelease();
        }
        #endregion
    }
}
