using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Common.Components
{
    public readonly struct Node(EntityLocalId localId, EntityLocalId treeLocalId) : IEntityComponent, IBelongsTo<Tree, Node>
    {
        public readonly EntityLocalId LocalId = localId;
        public readonly EntityLocalId TreeLocalId = treeLocalId;

        public readonly EntityFilterId<Node> TreeFilterId => new(this.TreeLocalId.Value);
        EntityFilterId<Node> IBelongsTo<Tree, Node>.ParentFilterId => this.TreeFilterId;
    }
}
