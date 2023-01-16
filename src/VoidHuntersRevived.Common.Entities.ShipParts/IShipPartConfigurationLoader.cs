using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts.Services;

namespace VoidHuntersRevived.Common.Entities.ShipParts
{
    public interface IShipPartConfigurationLoader
    {
        void Load(IShipPartConfigurationService shipParts);
    }
}
