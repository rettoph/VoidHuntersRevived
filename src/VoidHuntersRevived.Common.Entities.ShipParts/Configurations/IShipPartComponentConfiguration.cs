using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Configurations
{
    public interface IShipPartComponentConfiguration
    {
        void AttachComponentTo(Entity entity);
    }
}
