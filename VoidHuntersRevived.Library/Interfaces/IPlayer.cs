using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Connections.Nodes;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Library.Interfaces
{
    public interface IPlayer : INetworkEntity
    {
        String Name { get; }
        TractorBeam TractorBeam { get; }
        ShipPart Bridge { get; }

        FemaleConnectionNode[] AvailableFemaleConnectionNodes { get; }

        Boolean[] Movement { get; set; }

        void SetBridge(ShipPart bridge);
    }
}
