using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public struct GroupIndex
    {
        public readonly ExclusiveGroupStruct GroupID;
        public readonly uint Index;

        public GroupIndex(ExclusiveGroupStruct groupID, uint index)
        {
            this.GroupID = groupID;
            this.Index = index;
        }
    }
}
