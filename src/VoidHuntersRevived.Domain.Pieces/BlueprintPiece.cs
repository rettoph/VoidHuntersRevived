using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Domain.Pieces
{
    internal class BlueprintPiece : IBlueprintPiece
    {
        public Piece Type { get; }

        public BlueprintPiece(BlueprintPieceDto dto, IPieceService pieces)
        {
            if(!pieces.TryGetByKey(dto.Key, out Piece? piece))
            {
                throw new ArgumentException($"Unknown {nameof(Piece)}.{nameof(Piece.Key)} - {dto.Key}");
            }

            this.Type = piece;
        }

        // public EntityId Spawn(VhId treeId, Id<ITeam> teamId, IEntityService entities)
        // {
        //     VhId vhid = HashBuilder<BlueprintPieceDto, VhId, Id<BlueprintDto>>.Instance.Calculate(treeId, this.Blueprint.Id);
        // 
        //     return _pieceSpawner.Spawn(treeId, vhid, teamId, entities);
        // }
    }
}
