using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Weapons;
using Guppy;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Entities.Ammunitions
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
        #endregion

        #region Public Attributes
        public Vector2 LinearVelocity { get; private set; }
        public Vector2 StartPosition { get; private set; }
        public Single MaxDistance { get; private set; }
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


            // Save the projectiles initial starting positio n
            this.StartPosition = this.Position;

            this.MaxDistance = 50f;
            
            // Calculate an initial velcotity for the projectile
            this.LinearVelocity = this.Weapon.Root.LinearVelocity + Vector2.Transform(Vector2.UnitX * -15, Matrix.CreateRotationZ(this.Rotation));
        }
        #endregion

        #region Frame Methods 
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Step(this.LinearVelocity * ((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000));

            if (this.MaxDistance <= Vector2.Distance(this.StartPosition, this.Position))
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
            // this.Position += delta;
            _world.RayCast(this.HandleRayCastCollision, this.Position, this.Position += delta);
        }

        private float HandleRayCastCollision(Fixture arg1, Vector2 arg2, Vector2 arg3, float arg4)
        {
            var target = arg1.UserData as ShipPart;
            if (arg1.UserData is ShipPart && target.Root.Id != this.Weapon.Root.Id && target.Health > 0 && target.Root.IsBridge)
            {
                (arg1.UserData as ShipPart).Health -= 10;
                this.Dispose();
                return 0;
            }
            
            return -1;
        }
        #endregion
    }
}
