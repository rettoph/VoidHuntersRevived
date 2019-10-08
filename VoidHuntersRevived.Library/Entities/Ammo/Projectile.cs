using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using GalacticFighters.Library.Entities.ShipParts.Weapons;
using Guppy;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Entities.Ammo
{
    /// <summary>
    /// Projectile's represent ammo that moves
    /// and does not instantly cast a forward ray.
    /// 
    /// Projectiles thus, must have amoving body.
    /// </summary>
    public class Projectile : Entity
    {
        #region Private Fields
        private World _world;
        private Double _life;
        private Body _body;
        #endregion

        #region Public Attributes
        public Gun Gun { get; internal set; }
        #endregion

        #region Constructor
        public Projectile(World world)
        {
            _world = world;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();
            _life = 0;

            _body = BodyFactory.CreateCircle(_world, 0.1f, 0.1f, this.Gun.WorldBodyAnchor, BodyType.Dynamic);
            _body.IsSensor = true;

            // The target radian the gun will shoot towards
            var target = this.Gun.Rotation + this.Gun.JointAngle;
            // The difference between the current velocity and the guns target radian
            var offset = (Single)Math.Atan2(this.Gun.Root.LinearVelocity.Y, this.Gun.Root.LinearVelocity.X) - target;
            // The amount of forward thrust the bullet should be given, as a multiplier of the gun's current velocity
            var boost = (Single)Math.Cos(offset + MathHelper.Pi);
            // An inverse 0 through 1, used for the true modifier
            var modifier = 0.5f * (Single)Math.Cos(offset) + 0.5f;

            this.logger.LogInformation(modifier.ToString("#.##0"));

            // Set the bullets brand new velocity.. complete with the velocity multiplier
            _body.LinearVelocity = (this.Gun.Root.LinearVelocity * modifier) + Vector2.Transform(Vector2.UnitX * (-10 - (this.Gun.Root.LinearVelocity.Length() * boost)), Matrix.CreateRotationZ(target));
        }

        public override void Dispose()
        {
            base.Dispose();

            _body.Dispose();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _life += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_life > 5000)
                this.Dispose();
        }
        #endregion
    }
}
