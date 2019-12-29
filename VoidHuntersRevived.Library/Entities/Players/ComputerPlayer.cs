using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Entities.Players
{
    public class ComputerPlayer : Player
    {
        #region Public Properties
        public override String Name => "Computer";
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(this.Ship?.Bridge != default(ShipPart))
            {
                var nearest = this.players
                    .Where(p => p.Id != this.Id && p.Team != this.Team && p.Ship?.Bridge != default(ShipPart) && Vector2.Distance(this.Ship.Bridge.WorldCenter, p.Ship.Bridge.WorldCenter) < 100)
                    .OrderBy(p => Vector2.Distance(this.Ship.Bridge.WorldCenter, p.Ship.Bridge.WorldCenter))
                    .FirstOrDefault();

                if (nearest != default(Player))
                { // Only proceed if the target isnt null...
                    var offset = nearest.Ship.Bridge.WorldCenter - this.Ship.Bridge.WorldCenter;

                    // Re-position the current ship towards the target player...
                    var targetRotation = (Single)Math.Atan2(offset.Y, offset.X);
                    var targetRotationDifference = MathHelper.WrapAngle(this.Ship.Bridge.Rotation - targetRotation);

                    // Change the rotation
                    if (targetRotationDifference < -0.1)
                    { // Turn right...
                        this.Ship.SetDirection(Ship.Direction.TurnLeft, false);
                        this.Ship.SetDirection(Ship.Direction.TurnRight, true);
                    }
                    else if (targetRotationDifference > 0.1)
                    { // Turn left...
                        this.Ship.SetDirection(Ship.Direction.TurnLeft, true);
                        this.Ship.SetDirection(Ship.Direction.TurnRight, false);
                    }
                    else
                    { // Not turn at all...
                        this.Ship.SetDirection(Ship.Direction.TurnLeft, false);
                        this.Ship.SetDirection(Ship.Direction.TurnRight, false);
                    }

                    // Change the velocity
                    var distace = Vector2.Distance(this.Ship.Bridge.WorldCenter, nearest.Ship.Bridge.WorldCenter);
                    if (distace > 30 && Math.Abs(targetRotationDifference) < MathHelper.PiOver4)
                    {
                        this.Ship.SetDirection(Ship.Direction.Forward, true);
                        this.Ship.SetDirection(Ship.Direction.Backward, false);
                    }
                    else if (distace < 25 && Math.Abs(targetRotationDifference) < MathHelper.PiOver4)
                    {
                        this.Ship.SetDirection(Ship.Direction.Forward, false);
                        this.Ship.SetDirection(Ship.Direction.Backward, true);
                    }
                    else
                    {
                        this.Ship.SetDirection(Ship.Direction.Forward, false);
                        this.Ship.SetDirection(Ship.Direction.Backward, false);
                    }


                    // Update the ships target...
                    this.Ship.SetTarget(offset);

                    if (distace < 40f)
                        this.Ship.SetFiring(true);
                    else
                        this.Ship.SetFiring(false);
                }
                else
                {
                    this.Ship.SetFiring(false);
                    this.Ship.SetDirection(Ship.Direction.Forward, false);
                    this.Ship.SetDirection(Ship.Direction.Backward, false);
                    this.Ship.SetDirection(Ship.Direction.TurnLeft, false);
                    this.Ship.SetDirection(Ship.Direction.TurnRight, false);
                }
            }
        }
        #endregion
    }
}
