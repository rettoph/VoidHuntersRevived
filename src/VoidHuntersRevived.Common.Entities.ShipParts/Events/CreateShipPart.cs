using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Events;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Events
{
    public class CreateShipPart : CreateEntity
    {
        public readonly string Configuration;

        public CreateShipPart(ParallelKey key, string configuration) : base(key)
        {
            this.Configuration = configuration;
        }
    }
}
