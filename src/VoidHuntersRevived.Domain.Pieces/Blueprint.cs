using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Services;

namespace VoidHuntersRevived.Domain.Pieces
{
    internal class Blueprint : IBlueprint
    {
        private readonly BlueprintDto _dto;

        public Id<IBlueprint> Id { get; }

        public string Name { get; }

        public IBlueprintPiece Head { get; }

        public Blueprint(BlueprintDto dto, IPieceService pieces)
        {
            _dto = dto;

            this.Id = _dto.Id;
            this.Name = _dto.Name;
            this.Head = new BlueprintPiece(dto.Head, pieces);
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
