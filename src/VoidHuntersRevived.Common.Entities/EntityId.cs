using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities
{
    public struct EntityId : IEntityComponent
    {
        /// <summary>
        /// Svelto's EGID, non deterministic.
        /// </summary>
        public readonly EGID EGID;

        /// <summary>
        /// Determinstic internal id
        /// </summary>
        public readonly VhId VhId;

        public EntityId(EGID eGID, VhId vhId)
        {
            this.EGID = eGID;
            this.VhId = vhId;
        }
    }
}
