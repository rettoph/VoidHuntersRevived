using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Utilities;

namespace VoidHuntersRevived.Domain.Pieces.Common.Components
{
    public readonly struct Tree(EntityId treeId, EntityId headId) : IEntityComponent, IHasMany<Node>
    {
        public static readonly FilterContextID NodeFilterContextId = FilterContextID.GetNewContextID();

        public readonly EntityId HeadId = headId;

        public EntityFilterId<Node> ChildrenFilterId { get; } = new EntityFilterId<Node>(treeId.EGID);
    }
}
