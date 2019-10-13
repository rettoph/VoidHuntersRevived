using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalacticFighters.Library.Entities.ShipParts;
using GalacticFighters.Library.Scenes;
using Microsoft.Xna.Framework;

namespace GalacticFighters.Library.Entities.Players
{
    public class ComputerPlayer : Player
    {
        public override string Name { get => "Computer"; }

        public ComputerPlayer(GalacticFightersWorldScene scene) : base(scene)
        {
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.Ship?.Bridge != default(ShipPart))
            {
                var nearest = this.scene.Players
                    .Where(p => p.Id != this.Id && p.Ship?.Bridge != default(ShipPart) && Vector2.Distance(this.Ship.Bridge.WorldCenter, p.Ship.Bridge.WorldCenter) <= 500)
                    .OrderBy(p => Vector2.Distance(this.Ship.Bridge.WorldCenter, p.Ship.Bridge.WorldCenter))
                    .FirstOrDefault();

                if (nearest != default(Player))
                { // If there is a valid target in range...
                    // Re-position the current ship towars the target player
                    var targetRotation = Math.Atan2(nearest.Ship.Bridge.WorldCenter.Y - this.Ship.Bridge.Position.Y, nearest.Ship.Bridge.WorldCenter.X - this.Ship.Bridge.Position.X);
                    var targetRotationDifference = this.Ship.Bridge.Rotation - targetRotation;

                    // Change rotation
                    if (targetRotationDifference < -0.1f)
                    {
                        this.Ship.SetDirection(Ship.Direction.TurnLeft, false);
                        this.Ship.SetDirection(Ship.Direction.TurnRight, true);
                    }
                    else if (targetRotationDifference > 0.1f)
                    {
                        this.Ship.SetDirection(Ship.Direction.TurnLeft, true);
                        this.Ship.SetDirection(Ship.Direction.TurnRight, false);
                    }
                    else
                    {
                        this.Ship.SetDirection(Ship.Direction.TurnLeft, false);
                        this.Ship.SetDirection(Ship.Direction.TurnRight, false);
                    }

                    // Change forward velocity
                    var distance = Vector2.Distance(nearest.Ship.Bridge.WorldCenter, this.Ship.Bridge.Position);
                    if (distance > 20)
                    {
                        this.Ship.SetDirection(Ship.Direction.Forward, true);
                        this.Ship.SetDirection(Ship.Direction.Backward, false);
                    }
                    else if (distance < 10)
                    {
                        this.Ship.SetDirection(Ship.Direction.Forward, false);
                        this.Ship.SetDirection(Ship.Direction.Backward, true);
                    }
                    else
                    {
                        this.Ship.SetDirection(Ship.Direction.Forward, false);
                        this.Ship.SetDirection(Ship.Direction.Backward, false);
                    }

                    // Update the current player's target
                    this.Ship.SetTargetOffset(nearest.Ship.Bridge.WorldCenter - this.Ship.Bridge.WorldCenter);

                    // Set the fire status
                    if (distance < 50f)
                        this.Ship.SetFiring(true);
                }
                else
                {
                    this.Ship.SetFiring(false);
                }
            }
        }
    }
}
