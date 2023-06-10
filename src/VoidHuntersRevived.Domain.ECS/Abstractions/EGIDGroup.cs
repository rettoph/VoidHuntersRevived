using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Domain.ECS.Abstractions
{
    internal readonly struct EGIDGroup
    {
        public readonly EGID EGID;
        public readonly ExclusiveGroup Group;

        public EGIDGroup(EGID eGID, ExclusiveGroup group)
        {
            EGID = eGID;
            Group = group;
        }
    }
}
