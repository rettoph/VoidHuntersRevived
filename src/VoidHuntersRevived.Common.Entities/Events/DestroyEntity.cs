using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Common.Entities.Events
{
    public sealed class DestroyEntity : IEventData
    {
        public required VhId VhId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<DestroyEntity, VhId, VhId>.Instance.Calculate(in source, this.VhId);
        }
    }
}
