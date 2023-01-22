using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Events
{
    public sealed class CleanJointed : IData
    {
        public readonly Jointing Jointed;

        public CleanJointed(Jointing jointed)
        {
            Jointed = jointed;
        }
    }
}
