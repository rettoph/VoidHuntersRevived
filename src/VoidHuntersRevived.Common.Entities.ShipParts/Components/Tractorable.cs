using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Components
{
    public sealed class Tractorable
    {
        public static Tractorable Instance = new Tractorable(null);

        public int? WhitelistedTractoring;

        public Tractorable(int? whitelistedTractoring)
        {
            WhitelistedTractoring = whitelistedTractoring;
        }
    }
}
