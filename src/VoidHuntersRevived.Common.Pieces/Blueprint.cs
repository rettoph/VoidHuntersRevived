using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Core.Utilities;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Pieces.Extensions;

namespace VoidHuntersRevived.Common.Pieces
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

        // public EntityId Spawn(VhId treeId, VhId vhid, Id<ITeam> teamId, IEntityService entities)
        // {
        //     return entities.Spawn(_piece.EntityType, vhid, teamId, (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
        //     {
        //         initializer.Init(new Node(id, entities.GetId(treeId)));
        //     });
        // }
    }
}
