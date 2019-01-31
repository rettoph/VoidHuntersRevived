using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts.Hulls;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Library.Interfaces
{
    public interface IPlayer : INetworkEntity
    {
        String Name { get; }
        TractorBeam TractorBeam { get; }
        Hull Bridge { get; }

        Boolean[] Movement { get; set; }

        void SetBridge(Hull bridge);
    }
}
