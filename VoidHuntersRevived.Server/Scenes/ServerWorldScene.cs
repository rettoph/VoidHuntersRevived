using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Server.Scenes
{
    public class ServerWorldScene : WorldScene
    {
        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            // Create a new ship part
            var sp = this.entities.Create<Ship>("entity:ship", s =>
            {
                s.SetBridge(this.entities.Create<ShipPart>("entity:ship-part"));
            });
        }
        #endregion
    }
}
