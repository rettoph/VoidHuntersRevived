using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Connections.Nodes;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public partial class ShipPart
    {
        public virtual void HandleMaleNodeConnected(object sender, ConnectionNode e)
        {
            this.UpdateFixture();
        }

        public virtual void HandleMaleNodeDisconneced(object sender, ConnectionNode e)
        {
            // throw new NotImplementedException();
        }
    }
}
