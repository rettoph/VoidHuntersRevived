using Guppy;
using Guppy.Attributes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Server.Drivers.Entities.Players
{
    [IsDriver(typeof(ComputerPlayer))]
    internal sealed class ComputerPlayerServerDriver : Driver<ComputerPlayer>
    {
        #region Constructor
        public ComputerPlayerServerDriver(ComputerPlayer driven) : base(driven)
        {
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(this.driven.Ship.Bridge == default(ShipPart))
            {
                this.driven.Ship.Dispose();
                this.driven.Dispose();
            }
        }
        #endregion

        #region Lifecycle Methods
        #endregion
    }
}
