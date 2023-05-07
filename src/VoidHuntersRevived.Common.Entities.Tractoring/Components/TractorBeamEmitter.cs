using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Tractoring.Components
{
    public class TractorBeamEmitter
    {
        public readonly int EntityId;

        public bool Online { get; set; }
        public TractorBeam? TractorBeam { get; set; }

        public TractorBeamEmitter(int entityId)
        {
            this.EntityId = entityId;
        }
    }
}
