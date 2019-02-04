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

            this.Body.CollidesWith = FarseerPhysics.Dynamics.Category.Cat1;
            this.Body.CollisionCategories = FarseerPhysics.Dynamics.Category.Cat10;

            this.Root.Body.CollidesWith = FarseerPhysics.Dynamics.Category.Cat1;
            this.Root.Body.CollisionCategories = FarseerPhysics.Dynamics.Category.Cat10;
        }

        internal void HandleMaleConnectionNodeDisonnected(object sender, ConnectionNode node)
        {
            this.UpdateChainPlacement();

            this.Body.CollidesWith = FarseerPhysics.Dynamics.Category.Cat1;
            this.Body.CollisionCategories = FarseerPhysics.Dynamics.Category.Cat10;
        }
    }
}
