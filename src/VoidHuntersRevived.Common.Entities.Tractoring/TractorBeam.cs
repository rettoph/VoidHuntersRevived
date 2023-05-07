using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Tractoring.Components;

namespace VoidHuntersRevived.Common.Entities.Tractoring
{
    public class TractorBeam
    {
        public readonly Tractorable Target;

        public TractorBeam(Tractorable target)
        {
            this.Target = target;
        }
    }
}
