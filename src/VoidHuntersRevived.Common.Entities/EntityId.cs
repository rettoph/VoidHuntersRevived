using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities
{
    public struct EntityId
    {
        public readonly EGID EGID;
        public readonly VhId VhId;
        public bool Destroyed;

        public EntityId(EGID eGID, VhId vhId)
        {
            this.EGID = eGID;
            this.VhId = vhId;
            this.Destroyed = false;
        }
    }
}
