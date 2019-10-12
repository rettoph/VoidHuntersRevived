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

            if(this.Ship?.Bridge != default(ShipPart))
            {
                var nearest = this.scene.Players
                    .Where(p => p.Id != this.Id && p.Ship?.Bridge != default(ShipPart) && !(p is ComputerPlayer) && Vector2.Distance(this.Ship.Bridge.WorldCenter, p.Ship.Bridge.WorldCenter) < 45)
                    .OrderBy(p => Vector2.Distance(this.Ship.Bridge.WorldCenter, p.Ship.Bridge.WorldCenter))
                    .FirstOrDefault();

                if(nearest != default(Player))
                { // If there is a valid target in range...
                    // Update the current player's target
                    this.Ship.SetTargetOffset(nearest.Ship.Bridge.WorldCenter - this.Ship.Bridge.WorldCenter);

                    // Set the fire status
                    this.Ship.SetFiring(true);
                }
                else {
                    this.Ship.SetFiring(false);
                }
            }
        }
    }
}
