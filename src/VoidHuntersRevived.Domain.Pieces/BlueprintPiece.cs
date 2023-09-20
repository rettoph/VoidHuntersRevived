using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Domain.Pieces
{
    internal class BlueprintPiece : IBlueprintPiece
    {
        public Piece Piece { get; }

        public IBlueprintPiece[][] Children { get; }

        public BlueprintPiece(BlueprintPieceDto dto, IPieceService pieces)
        {
            if(!pieces.TryGetByKey(dto.Key, out Piece? piece))
            {
                throw new ArgumentException($"Unknown {nameof(Common.Pieces.Piece)}.{nameof(Common.Pieces.Piece.Key)} - {dto.Key}");
            }

            this.Piece = piece;
            this.Children = BlueprintPiece.BuildChildren(dto, this.Piece, pieces);
        }

        private static IBlueprintPiece[][] BuildChildren(BlueprintPieceDto dto, Piece piece, IPieceService pieces)
        {
            bool descriptorHasSockets = piece.Descriptor.ComponentManagers.Any(x => x.Type == typeof(Sockets<Location>));
            bool dtoHasChildren = dto.Children?.Any() ?? false;

            if (descriptorHasSockets == false && dtoHasChildren == true)
            {
                throw new ArgumentException($"Children and Socket mismatch. DescriptorHasSockets = {descriptorHasSockets}, BlueprintPieceDtoHasChldren = {dtoHasChildren}, {nameof(BlueprintPieceDto)}.{nameof(BlueprintPieceDto.Key)} = {dto.Key}");
            }

            if (descriptorHasSockets == true && dtoHasChildren == false)
            {
                return Array.Empty<IBlueprintPiece[]>();
            }

            if(descriptorHasSockets == false)
            {
                return Array.Empty<IBlueprintPiece[]>();
            }

            Sockets<Location> sockets = piece.Components.Values.OfType<Sockets<Location>>().SingleOrDefault();

            if(sockets.Items.count < dto.Children!.Length)
            {
                throw new ArgumentException($"Too many children defined. Expected {sockets.Items.count} and found {dto.Children.Length}, {nameof(BlueprintPieceDto)}.{nameof(BlueprintPieceDto.Key)} = {dto.Key}");
            }

            IBlueprintPiece[][] children = new IBlueprintPiece[sockets.Items.count][];

            for(int i=0; i<sockets.Items.count; i++)
            {
                if(dto.Children.Length <= i || dto.Children[i] is null)
                {
                    children[i] = Array.Empty<IBlueprintPiece>();
                    continue;
                }

                children[i] = dto.Children[i]!
                    .Select(x => new BlueprintPiece(x, pieces))
                    .ToArray();
            }

            return children;
        }

        // public EntityId Spawn(VhId treeId, Id<ITeam> teamId, IEntityService entities)
        // {
        //     VhId vhid = HashBuilder<BlueprintPieceDto, VhId, Id<BlueprintDto>>.Instance.Calculate(treeId, this.Blueprint.Id);
        // 
        //     return _pieceSpawner.Spawn(treeId, vhid, teamId, entities);
        // }
    }
}
