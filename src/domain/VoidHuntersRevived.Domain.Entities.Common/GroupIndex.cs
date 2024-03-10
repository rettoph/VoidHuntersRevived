using Svelto.ECS;

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
