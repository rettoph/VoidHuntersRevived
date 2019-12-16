using Guppy;
using Guppy.Attributes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Server.Drivers.Entities.Players
{
    [IsDriver(typeof(ComputerPlayer))]
    internal sealed class ComputerPlayerServerDriver : Driver<ComputerPlayer>
    {
        public ComputerPlayerServerDriver(ComputerPlayer driven) : base(driven)
        {
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(this.driven.Ship.Bridge == default(ShipPart))
            {
                this.driven.Ship.Dispose();
                this.driven.Dispose();
            }
        }
    }
}
