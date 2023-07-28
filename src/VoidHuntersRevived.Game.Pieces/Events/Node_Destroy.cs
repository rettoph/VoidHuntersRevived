using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Events;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Game.Pieces.Events
{
    internal class Node_Destroy : IEventData
    {
        public required VhId TreeId { get; init; }
        public required VhId NodeId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<Node_Destroy, VhId, VhId, VhId>.Instance.Calculate(source, this.NodeId, this.TreeId);
        }
    }
}
