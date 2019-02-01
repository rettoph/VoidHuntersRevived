using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Connections.Nodes;
using VoidHuntersRevived.Library.Entities.Interfaces;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Client.Entities.Drivers
{
    public class ClientShipPartDriver : ClientFarseerEntityDriver
    {
        private ShipPart _parent;

        public ClientShipPartDriver(ShipPart parent, EntityInfo info, IGame game) : base(parent, info, game)
        {
            _parent = parent;

            _parent.OnInitialize += this.HandleParentInitialize;
        }

        private void HandleParentInitialize(object sender, IInitializable e)
        {
            _parent.MaleConnectionNode.OnConnected += this.HandleMaleNodeConnection;
            _parent.MaleConnectionNode.OnDisconnected += this.HandleMaleNodeDisconneced;
        }

        private void HandleMaleNodeConnection(object sender, ConnectionNode e)
        {
            // throw new NotImplementedException();
        }

        private void HandleMaleNodeDisconneced(object sender, ConnectionNode e)
        {
            // throw new NotImplementedException();
        }
    }
}
