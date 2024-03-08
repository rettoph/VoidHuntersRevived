using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Pieces
{
    public sealed class Blueprint : IEntityResource<Blueprint>
    {
        Id<Blueprint> IEntityResource<Blueprint>.Id => this.Id;

        public readonly Id<Blueprint> Id;
        public readonly string Name;
        public readonly IBlueprintPiece Head;

        public Blueprint(Id<Blueprint> id, string name, IBlueprintPiece head)
        {
            this.Id = id;
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
