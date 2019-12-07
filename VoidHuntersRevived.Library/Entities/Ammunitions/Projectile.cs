using FarseerPhysics.Dynamics;
using Guppy;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Entities.Ammunitions
{
    /// <summary>
    /// A simple type of ammunition that travels along
    /// a designated direction & checks for collisions via
    /// raycast each frame.
    /// </summary>
    public class Projectile : Ammunition
    {
        #region Private Fields
        private World _world;
        private ShipPart _target;
        #endregion

        #region Public Attributes
        public Vector2 Velocity { get; protected internal set; }
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
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Step(this.Velocity * ((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000));
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Step by a given amount & check for any collisions
        /// </summary>
        /// <param name="delta"></param>
        private void Step(Vector2 delta)
        {
            _world.RayCast(this.HandleRayCastCollision, this.Position, this.Position += delta);
        }
        #endregion

        #region Event Handlers
        private float HandleRayCastCollision(Fixture arg1, Vector2 arg2, Vector2 arg3, float arg4)
        {
            if(arg1.UserData is ShipPart && (_target = arg1.UserData as ShipPart).Root != this.Weapon.Root && _target.Root.Ship != default(Ship))
            { // If the projectile collides with a ship part from another chain that is not the origin chain...

                this.Dispose();
                return 0;
            }

            return -1;
        }
        #endregion
    }
}
