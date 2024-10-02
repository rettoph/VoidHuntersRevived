using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public struct GroupIndex(ExclusiveGroupStruct groupID, uint index)
    {
        public readonly ExclusiveGroupStruct GroupID = groupID;
        public readonly uint Index = index;
    }
}
