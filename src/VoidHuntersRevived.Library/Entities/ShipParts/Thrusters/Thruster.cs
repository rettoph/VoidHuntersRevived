using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Contexts.ShipParts;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Thrusters
{
    public class Thruster : RigidShipPart<ThrusterContext>
    {
        /// <summary>
        /// Return a <see cref="Direction"/>
        /// flags that the current <see cref="Thruster"/> will 
        /// move its owning <see cref="Chain"/>, if any.
        /// </summary>
        /// <returns></returns>
        public Direction GetDirections()
        {
            Direction flags = Direction.None;

            if(this.Chain is not null)
            {
                // Each andle of movement has a buffer sone on inclusivity
                var buffer = 0.01f;

                // The chain's center of mass
                var com = this.Chain.Body.LocalCenter;
                // The point acceleration is applied
                var ip = this.LocalCenter;
                // The impulse to be applied...
                var i = Vector2.Transform(this.Context.MaximumThrust, Matrix.CreateRotationZ(this.LocalRotation));
                // The point acceleration is targeting
                var it = ip + i;

                // The angle between the com and the acceleration point
                var ipr = MathHelper.WrapAngle((float)Math.Atan2(ip.Y - com.Y, ip.X - com.X));
                // The angle between the com and the acceleration target
                var itr = MathHelper.WrapAngle((float)Math.Atan2(it.Y - com.Y, it.X - com.X));
                // The angle between the acceleration point and the acceleration target
                var ipitr = MathHelper.WrapAngle((float)Math.Atan2(it.Y - ip.Y, it.X - ip.X));
                // The relative acceleration target rotation between the acceleration point and center of mass
                var ript = MathHelper.WrapAngle(ipitr - ipr);

                // Define some lower and upper bounds...
                var ipitr_lower = ipitr - buffer;
                var ipitr_upper = ipitr + buffer;

                // Check if the thruster moves the chain forward...
                if ((ipitr_upper < MathHelper.PiOver2 && ipitr_lower > -MathHelper.PiOver2))
                    flags |= Direction.Forward;

                // Check if the thruster turns the chain right...
                if (ript > 0)
                    flags |= Direction.TurnRight;

                // Check if the thruster moves the chain backward...
                if (ipitr_lower > MathHelper.PiOver2 || ipitr_upper < -MathHelper.PiOver2)
                    flags |= Direction.Backward;

                // Check if the thruster turns the chain left...
                if (ript < 0)
                    flags |= Direction.TurnLeft;

                // Check if the thruster moves the chain right...
                if (ipitr_lower < 0 && ript > -MathHelper.Pi)
                    flags |= Direction.Right;

                // Check if the thruster moves the chain left...
                if (ipitr_lower > 0 && ipitr_upper < MathHelper.Pi)
                    flags |= Direction.Left;
            }

            return flags;
        }
    }
}
