using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Pieces.Common.Components
{
    public readonly struct Node(EntityLocalId localId, EntityLocalId treeLocalId) : IEntityComponent
    {
        public readonly EntityLocalId LocalId = localId;
        public readonly EntityLocalId TreeLocalId = treeLocalId;
    }
}