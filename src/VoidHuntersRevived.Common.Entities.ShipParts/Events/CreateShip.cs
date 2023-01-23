using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Events;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Events
{
    public class CreateShip : CreateEntity
    {
        public string BridgeConfiguration;

        public CreateShip(ParallelKey key, string bridgeConfiguration) : base(key)
        {
            this.BridgeConfiguration = bridgeConfiguration;
        }
    }
}
