using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Components
{
    public sealed class Tractorable
    {
        public static readonly Tractorable Instance = new Tractorable();

        private Tractorable()
        {

        }
    }
}
