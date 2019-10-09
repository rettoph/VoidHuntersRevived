using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using GalacticFighters.Library.Entities.ShipParts;
using GalacticFighters.Library.Entities.ShipParts.Weapons;
using Guppy;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Entities.Ammunitions
{
    /// <summary>
    /// Projectile's represent ammo that moves
    /// and does not instantly cast a forward ray.
    /// 
    /// Projectiles thus, must have amoving body.
    /// </summary>
    public class Projectile : Ammunition
    {
        #region Private Fields
        private World _world;
        private Double _life;
        #endregion

        #region Public Attributes
        public Vector2 LinearVelocity { get; private set; }
        #endregion

        #region Constructors
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

            // The target radian the gun will shoot towards
            // The difference between the current velocity and the guns target radian
            var offset = (Single)Math.Atan2(this.Weapon.Root.LinearVelocity.Y, this.Weapon.Root.LinearVelocity.X) - this.Rotation;
            // The amount of forward thrust the bullet should be given, as a multiplier of the gun's current velocity
            var boost = (Single)Math.Cos(offset + MathHelper.Pi);
            // An inverse 0 through 1, used for the true modifier
            var modifier = 0.5f * (Single)Math.Cos(2 * offset + MathHelper.Pi) + 0.5f;

            // Set the bullets brand new velocity.. complete with the velocity multiplier
            this.LinearVelocity = (this.Weapon.Root.LinearVelocity * modifier) + Vector2.Transform(Vector2.UnitX * (-10 - (this.Weapon.Root.LinearVelocity.Length() * boost)), Matrix.CreateRotationZ(this.Rotation));
        }
        #endregion

        #region Frame Methods 
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Step(this.LinearVelocity * ((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000));

            _life += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_life > 5000)
                this.Dispose();
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Step the projectile toward a specified target,
        /// and raw cast the world to find any possible
        /// colissions.
        /// </summary>
        /// <param name="delta"></param>
        private void Step(Vector2 delta)
        {
            _world.RayCast(this.HandleRayCastCollision, this.Position, this.Position += delta);
        }

        private float HandleRayCastCollision(Fixture arg1, Vector2 arg2, Vector2 arg3, float arg4)
        {
            // this.logger.LogInformation((arg1.UserData as ShipPart)?.Root.Id.ToString());
            // if((arg1.UserData as ShipPart)?.Root != this.Weapon.Root)
            //     this.logger.LogInformation("collision!");

            return -1;
        }
        #endregion
    }
}
