using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Thrusters
{
    public class Thruster : RigidShipPart
    {
        #region Public Properties
        /// <summary>
        /// The directions the current thruster will
        /// update its chain.
        /// </summary>
        public Ship.Direction Directions { get; private set; }
        /// <summary>
        /// Indicates if the thruster was activeted this frame.
        /// </summary>
        public Boolean Active { get; internal set; }
        public Vector2 Thrust { get => Vector2.UnitX * 20; }
        public Vector2 LocalThrust { get => Vector2.Transform(this.Thrust, Matrix.CreateRotationZ(this.LocalRotation)); }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.DefaultColor = Color.ForestGreen;

            this.OnChainUpdated += this.HandleChainUpdated;
        }

        public override void Dispose()
        {
            base.Dispose();

            this.OnChainUpdated -= this.HandleChainUpdated;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Apply thrust to the internal fixture...
            this.ApplyThrust(this.Root.Body);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Returns all directions the current Thruster will
        /// move its chain when activated.
        /// </summary>
        /// <param name="directions"></param>
        /// <returns></returns>
        private Ship.Direction GetDirections()
        {
            // Each andle of movement has a buffer sone on inclusivity
            var buffer = 0.01f;

            // The chain's center of mass
            var com = this.Root.LocalCenter;
            // The point acceleration is applied
            var ap = this.LocalCenter;
            // The point acceleration is targeting
            var at = ap + this.LocalThrust;

            // The angle between the com and the acceleration point
            var apr = MathHelper.WrapAngle((float)Math.Atan2(ap.Y - com.Y, ap.X - com.X));
            // The angle between the com and the acceleration target
            var atr = MathHelper.WrapAngle((float)Math.Atan2(at.Y - com.Y, at.X - com.X));
            // The angle between the acceleration point and the acceleration target
            var apatr = MathHelper.WrapAngle((float)Math.Atan2(at.Y - ap.Y, at.X - ap.X));
            // The relative acceleration target rotation between the acceleration point and center of mass
            var ratr = MathHelper.WrapAngle(apatr - apr);

            var apatr_lower = apatr - buffer;
            var apatr_upper = apatr + buffer;

            Ship.Direction directions = Ship.Direction.None;

            // Check if the thruster moves the chain forward...
            if ((apatr_upper < MathHelper.PiOver2 && apatr_lower > -MathHelper.PiOver2))
                directions |= Ship.Direction.Forward;

            // Check if the thruster turns the chain right...
            if (ratr > 0)
                directions |= Ship.Direction.TurnRight;

            // Check if the thruster moves the chain backward...
            if (apatr_lower > MathHelper.PiOver2 || apatr_upper < -MathHelper.PiOver2)
                directions |= Ship.Direction.Backward;

            // Check if the thruster turns the chain left...
            if (ratr < 0)
                directions |= Ship.Direction.TurnLeft;

            // Check if the thruster moves the chain right...
            if (apatr_lower < 0 && ratr > -MathHelper.Pi)
                directions |= Ship.Direction.Right;
            
            // Check if the thruster moves the chain left...
            if (apatr_lower > 0 && apatr_upper < MathHelper.Pi)
                directions |= Ship.Direction.Left;

            return directions;
        }

        /// <summary>
        /// Apply thrust to the inputed FarseerBody,
        /// assuming that it belongs to the current
        /// Thruster's chain.
        /// </summary>
        /// <param name="body"></param>
        public void ApplyThrust(Body body)
        {
            if (this.Active)
            { // Only apply any thrust if the current thruster is active...
                // Calculate the thrusters position on the recieved body...
                var point = body.Position + Vector2.Transform(Vector2.Zero, this.LocalTransformation * Matrix.CreateRotationZ(body.Rotation));
                // Calculate the thrust's world force relative to the recieved body...
                var force = Vector2.Transform(this.Thrust, Matrix.CreateRotationZ(body.Rotation + this.LocalRotation));

                // Apply thr thrust...
                body.ApplyForce(ref force, ref point);
            }
        }
        #endregion

        #region Event Handlers
        private void HandleChainUpdated(object sender, ChainUpdate arg)
        {
            this.Directions = this.GetDirections();
        }
        #endregion
    }
}
