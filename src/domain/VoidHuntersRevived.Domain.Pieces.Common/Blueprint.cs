using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Extensions;

namespace VoidHuntersRevived.Domain.Pieces.Common
{
    public sealed class Blueprint : IEntityResource<Blueprint>
    {
        private Id<Blueprint>? _id;

        public Id<Blueprint> Id => _id ??= HashBuilder<Blueprint, VhId, VhId>.Instance.CalculateId(VhId.HashString(this.Name), this.Head.CalculateHash());
        public readonly string Name;
        public readonly IBlueprintPiece Head;

        public Blueprint(string name, IBlueprintPiece head)
        {
            this.Name = name;
            this.Head = head;
        }

        // public EntityId Spawn(VhId treeId, VhId vhid, Id<ITeam> teamId, IEntityQueryService entities)
        // {
        //     return entities.Spawn(_piece.EntityType, vhid, teamId, (IEntityQueryService entities, IEntityType type, EntityId id, ref EntityInitializer initializer) =>
        //     {
        //         initializer.Init(new Node(id, entities.GetId(treeId)));
        //     });
        // }
    }
}
