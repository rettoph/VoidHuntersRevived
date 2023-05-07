using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Tractoring.Components
{
    public class Tractorable
    {
        public readonly int EntityId;

        public Tractorable(int entityId)
        {
            this.EntityId = entityId;
        }
    }
}
