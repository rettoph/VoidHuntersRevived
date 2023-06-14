using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Domain.Entities.Abstractions
{
    internal readonly struct EGIDIndex
    {
        public readonly EGID EGID;
        public readonly uint Index;

        public EGIDIndex(EGID eGID, uint index)
        {
            EGID = eGID;
            Index = index;
        }
    }
}
