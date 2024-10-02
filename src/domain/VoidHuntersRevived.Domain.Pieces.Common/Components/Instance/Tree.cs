using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Pieces.Common.Components.Instance
{
    public struct Tree(EntityId headId) : IEntityComponent
    {
        public static readonly FilterContextID NodeFilterContextId = FilterContextID.GetNewContextID();

        public readonly EntityId HeadId = headId;
    }
}
