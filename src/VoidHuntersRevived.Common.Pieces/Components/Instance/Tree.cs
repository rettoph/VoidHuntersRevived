using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Pieces.Components.Instance
{
    public struct Tree : IEntityComponent
    {
        public static readonly FilterContextID NodeFilterContextId = FilterContextID.GetNewContextID();

        public readonly EntityId HeadId;

        public Tree(EntityId headId)
        {
            HeadId = headId;
        }
    }
}
