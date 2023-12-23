using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Common.Pieces.Events
{
    public class Tree_Clean : IEventData
    {
        public bool IsPredictable => true;
        public bool IsLocalOnly { get; init; } = true;

        public required VhId TreeId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<Tree_Clean, VhId, VhId>.Instance.Calculate(source, this.TreeId);
        }
    }
}
