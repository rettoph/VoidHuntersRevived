using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Extensions.Entities;

namespace VoidHuntersRevived.Library.Entities.Players
{
    /// <summary>
    /// A simple implementation of Player that will 
    /// run a simple AI.
    /// </summary>
    public class ComputerPlayer : Player
    {
        public override String Name => "Computer";

        #region Private Fields
        private Player _target;
        #endregion

        #region Lifecycle Methods
        protected override void Release()
        {
            base.Release();

            _target = default;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.Ship.Bridge != default(ShipPart))
            {
                if(_target?.Ship?.Bridge == default || Vector2.Distance(this.Ship.Bridge.WorldCenter, _target.Ship.Bridge.WorldCenter) > 100)
                    _target = this.players
                        .Where(
                            p => p.Id != this.Id && p.Ship?.Bridge != default(ShipPart) 
                            && Vector2.Distance(this.Ship.Bridge.WorldCenter, p.Ship.Bridge.WorldCenter) < 10000
                            && p.Team != this.Team)
                        .OrderBy(p => Vector2.Distance(this.Ship.Bridge.WorldCenter, p.Ship.Bridge.WorldCenter))
                    .FirstOrDefault();

                if (_target != default(Player))
                { // Only proceed if the target isnt null...

                    // Attempt to launch fighters...
                    this.Ship.TryLaunchFighters(gameTime);

                    var offset = _target.Ship.Bridge.WorldCenter - this.Ship.Bridge.WorldCenter;

                    // Re-position the current ship towards the target player...
                    var targetRotation = (Single)Math.Atan2(offset.Y, offset.X);
                    var targetRotationDifference = MathHelper.WrapAngle(this.Ship.Bridge.Rotation - targetRotation);

                    // Change the rotation
                    if (targetRotationDifference < -0.2 && this.Ship.Bridge.AngularVelocity < -0.5f * targetRotationDifference)
                    { // Turn right...
                        this.Ship.TrySetDirection(Ship.Direction.TurnLeft, false);
                        this.Ship.TrySetDirection(Ship.Direction.TurnRight, true);
                    }
                    else if (targetRotationDifference > 0.2 && this.Ship.Bridge.AngularVelocity > -0.5f * targetRotationDifference)
                    { // Turn left...
                        this.Ship.TrySetDirection(Ship.Direction.TurnLeft, true);
                        this.Ship.TrySetDirection(Ship.Direction.TurnRight, false);
                    }
                    else
                    { // Not turn at all...
                        this.Ship.TrySetDirection(Ship.Direction.TurnLeft, false);
                        this.Ship.TrySetDirection(Ship.Direction.TurnRight, false);
                    }

                    // Change the velocity
                    var distace = Vector2.Distance(this.Ship.Bridge.WorldCenter, _target.Ship.Bridge.WorldCenter);
                    if (distace > 35 && Math.Abs(targetRotationDifference) < MathHelper.PiOver4)
                    {
                        this.Ship.TrySetDirection(Ship.Direction.Forward, true);
                        this.Ship.TrySetDirection(Ship.Direction.Backward, false);
                    }
                    else if (distace < 30 && Math.Abs(targetRotationDifference) < MathHelper.PiOver4)
                    {
                        this.Ship.TrySetDirection(Ship.Direction.Forward, false);
                        this.Ship.TrySetDirection(Ship.Direction.Backward, true);
                    }
                    else
                    {
                        this.Ship.TrySetDirection(Ship.Direction.Forward, false);
                        this.Ship.TrySetDirection(Ship.Direction.Backward, false);
                    }


                    // Update the ships target...
                    this.Ship.Target = _target.Ship.Bridge.WorldCenter;

                    if (distace < 40f)
                        this.Ship.Firing = true;
                    else
                        this.Ship.Firing = false;
                }
                else
                {
                    this.Ship.Firing = false;
                    this.Ship.TrySetDirection(Ship.Direction.Forward, false);
                    this.Ship.TrySetDirection(Ship.Direction.Backward, false);
                    this.Ship.TrySetDirection(Ship.Direction.Left, false);
                    this.Ship.TrySetDirection(Ship.Direction.Right, false);
                    this.Ship.TrySetDirection(Ship.Direction.TurnLeft, false);
                    this.Ship.TrySetDirection(Ship.Direction.TurnRight, false);
                }
            }
        }
        #endregion
    }
}
