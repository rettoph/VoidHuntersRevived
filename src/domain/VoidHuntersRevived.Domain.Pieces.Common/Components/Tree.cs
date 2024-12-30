using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Common.Components
{
    public readonly struct Tree(EntityLocalId treeLocalId, EntityLocalId headLocalId) : IEntityComponent, IHasMany<Node>
    {
        public readonly EntityLocalId HeadLocalId = headLocalId;

        public EntityFilterId<Node> ChildrenFilterId { get; } = new EntityFilterId<Node>(treeLocalId.Value);
    }
}
