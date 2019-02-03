using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ConnectionNodes;
using VoidHuntersRevived.Library.Entities.Connections;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    /// <summary>
    /// The ConnectionNodeHandler partial ship part class will handle
    /// ConnectionNode related methods.
    /// </summary>
    public partial class ShipPart
    {
        internal void HandleMaleConnectionNodeConnected(object sender, NodeConnection connection)
        {
            this.UpdateChainPlacement();
        }

        internal void HandleMaleConnectionNodeDisonnected(object sender, ConnectionNode node)
        {
            this.UpdateChainPlacement();
        }
    }
}
