using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts.ConnectionNodes;
using VoidHuntersRevived.Library.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Extensions.Logging;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Thrusters
{
    /// <summary>
    /// The basic thruster objects,
    /// represents a ship part that can
    /// apply thrust at the local center of part.
    /// </summary>
    public class Thruster : RigidShipPart
    {
        #region Private Fields
        private List<Ship.Direction> _directions;
        #endregion

        #region Public Attributes
        public Boolean Active { get; private set; }
        public Vector2 Thrust { get => Vector2.UnitX * 20; }
        public Vector2 LocalThrust { get => Vector2.Transform(this.Thrust, Matrix.CreateRotationZ(this.LocalRotation)); }
        public Vector2 WorldThrust { get => Vector2.Transform(this.Thrust, Matrix.CreateRotationZ(this.Rotation)); }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _directions = new List<Ship.Direction>();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Active = false;
        }
        #endregion

        #region Action Methods
        /// <summary>
        /// Attempt to move the current thruster by the supplied thdirectionrust value
        /// </summary>
        /// <param name="direction"></param>
        public void TryThrust(Ship.Direction direction)
        {
            if (this.GetActive(direction))
            {
                this.Active = true;
                this.Root.ApplyForce(this.WorldThrust, this.WorldCenteroid);
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Returns true if the current thruster would alter its chain
        /// in any way given.
        /// </summary>
        /// <param name="directions"></param>
        /// <returns></returns>
        public Boolean GetActive(Ship.Direction active)
        {
            // Each andle of movement has a buffer sone on inclusivity
            var buffer = 0.01f;

            // The chain's center of mass
            var com = this.Root.LocalCenter;
            // The point acceleration is applied
            var ap = this.LocalCenteroid;
            // The point acceleration is targeting
            var at = ap + this.LocalThrust;

            // The angle between the com and the acceleration point
            var apr = RadianHelper.Normalize((float)Math.Atan2(ap.Y - com.Y, ap.X - com.X));
            // The angle between the com and the acceleration target
            var atr = RadianHelper.Normalize((float)Math.Atan2(at.Y - com.Y, at.X - com.X));
            // The angle between the acceleration point and the acceleration target
            var apatr = RadianHelper.Normalize((float)Math.Atan2(at.Y - ap.Y, at.X - ap.X));
            // The relative acceleration target rotation between the acceleration point and center of mass
            var ratr = RadianHelper.Normalize(apatr - apr);

            var apatr_lower = apatr - buffer;
            var apatr_upper = apatr + buffer;

            // Check if the thruster moves the chain forward...
            if (active.HasFlag(Ship.Direction.Forward) && (apatr_upper < MathHelper.PiOver2 || apatr_lower > 3 * MathHelper.PiOver2))
                return true;

            // Check if the thruster turns the chain right...
            if (active.HasFlag(Ship.Direction.TurnRight) && ratr > 0 && ratr < MathHelper.Pi)
                return true;

            // Check if the thruster moves the chain backward...
            if (active.HasFlag(Ship.Direction.Backward) && apatr_lower > MathHelper.PiOver2 && apatr_upper < 3 * MathHelper.PiOver2)
                return true;

            // Check if the thruster turns the chain left...
            if (active.HasFlag(Ship.Direction.TurnLeft) && ratr > MathHelper.Pi && ratr < MathHelper.TwoPi)
                return true;

            // Check if the thruster moves the chain right...
            if (active.HasFlag(Ship.Direction.Right) && apatr_lower > MathHelper.Pi && apatr_upper < MathHelper.TwoPi)
                return true;

            // Check if the thruster moves the chain left...
            if (active.HasFlag(Ship.Direction.Left) && apatr_lower > 0 && apatr_upper < MathHelper.Pi)
                return true;

            // case Direction.StrafeRight:
            //     return (apatr_lower > 0 && apatr_upper < RadianHelper.PI);
            // case Direction.StrafeLeft:
            //     return (apatr_lower > RadianHelper.PI && apatr_upper < RadianHelper.TWO_PI);

            return false;
        }
        #endregion
    }
}
