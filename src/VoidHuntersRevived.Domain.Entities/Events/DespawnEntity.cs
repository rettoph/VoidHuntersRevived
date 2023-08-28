using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Entities.Events
{
    public sealed class DespawnEntity : IEventData
    {
        public bool IsPredictable => false;

        public required VhId VhId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<DespawnEntity, VhId, VhId>.Instance.Calculate(in source, this.VhId);
        }
    }
}
