using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Services
{
    public interface IShipPartService
    {
        void MakeShipPart(Entity entity, ShipPartResource resource);
        Entity CreateShipPart(string resource, ISimulationEvent @event);
        Entity CreateShipPart(ShipPartResource resource, ISimulationEvent @event);
    }
}
