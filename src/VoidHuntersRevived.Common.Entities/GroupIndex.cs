using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities
{
    public struct GroupIndex
    {
        public readonly ExclusiveGroupStruct GroupID;
        public readonly uint Index;

        public GroupIndex(ExclusiveGroupStruct groupID, uint index)
        {
            GroupID = groupID;
            Index = index;
        }
    }
}
