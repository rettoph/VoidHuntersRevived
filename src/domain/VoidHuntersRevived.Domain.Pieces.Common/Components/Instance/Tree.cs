using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Pieces.Common.Components.Instance
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
