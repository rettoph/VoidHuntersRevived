using Guppy;
using Guppy.Attributes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;

namespace VoidHuntersRevived.Server.Drivers.Entities.Players
{
    [IsDriver(typeof(ComputerPlayer))]
    public class ServerComputerPlayerDrivers : Driver<ComputerPlayer>
    {
        public ServerComputerPlayerDrivers(ComputerPlayer driven) : base(driven)
        {
        }

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.driven.Ship.Bridge == null)
            {
                this.driven.Ship.Dispose();
                this.driven.Dispose();
            }
        }
        #endregion
    }
}
