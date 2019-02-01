using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Connections.Nodes;
using VoidHuntersRevived.Library.Entities.Players;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public partial class ShipPart
    {
        public virtual void HandleMaleNodeConnected(object sender, ConnectionNode e)
        {
            this.UpdateFixture();

            if (this.Root.BridgeFor != null)
                (this.Root.BridgeFor as Player).UpdateAvailableFemaleConnectionNodes();
        }

        public virtual void HandleMaleNodeDisconneced(object sender, ConnectionNode e)
        {
            // throw new NotImplementedException();
        }
    }
}
