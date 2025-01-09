using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Pieces.Common.Components
{
    [Obsolete("To be removed")]
    public readonly struct Tree(EntityLocalId treeLocalId, EntityLocalId headLocalId) : IEntityComponent
    {
        public readonly EntityLocalId HeadLocalId = headLocalId;
    }
}