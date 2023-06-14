using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities
{
    public readonly struct IdMap
    {
        public readonly EGID EGID;
        public readonly VhId VhId;

        public IdMap(EGID eGID, VhId vhId)
        {
            this.EGID = eGID;
            this.VhId = vhId;
        }
    }
}
