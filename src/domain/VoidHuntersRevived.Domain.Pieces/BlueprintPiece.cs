using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Descriptors;

namespace VoidHuntersRevived.Domain.Pieces
{
    internal sealed class BlueprintPiece : IBlueprintPiece
    {
        private readonly string _pieceTypeKey;
        private readonly Lazy<IEntityTypeService> _entityTypes;
        private IEntityType<PieceDescriptor>? _pieceType;
        private bool _initialized;

        public IEntityType<PieceDescriptor> PieceType => _pieceType ??= this.InitializePieceType();
        public IBlueprintPiece[][] Children { get; }

        public BlueprintPiece(string pieceTypeKey, IBlueprintPiece[][] children, Lazy<IEntityTypeService> entityTypes)
        {
            _pieceTypeKey = pieceTypeKey;
            _entityTypes = entityTypes;

            this.Children = children;
        }

        private IEntityType<PieceDescriptor> InitializePieceType()
        {
            if (!_entityTypes.Value.TryGetByKey(_pieceTypeKey, out IEntityType? entityType))
            {
                throw new ArgumentException($"Unknown {nameof(IEntityType)} - {_pieceTypeKey}");
            }

            if (entityType is not IEntityType<PieceDescriptor> pieceType)
            {
                throw new ArgumentException($"Invalid {nameof(IEntityType)} - {_pieceTypeKey}");
            }

            if (this.Children!.Length > 0)
            {
                Sockets<Location> sockets = entityType.InstanceComponents.Values.OfType<Sockets<Location>>().First();

                if (sockets.Items.count != this.Children.Length)
                {
                    throw new ArgumentException($"Unexpected amount of children defined. Expected {sockets.Items.count} and found {this.Children.Length}, {nameof(BlueprintPiece)}.{nameof(BlueprintPiece.PieceType)} = {_pieceType.Key}");
                }
            }

            return pieceType;
        }

        // private static BlueprintPiece[][] BuildChildren(BlueprintPieceDto dto, PieceType piece, IPieceTypeService pieces)
        // {
        //     bool descriptorHasSockets = piece.Descriptor.ComponentManagers.Any(x => x.Type == typeof(Sockets<Location>));
        //     bool dtoHasChildren = dto.Children?.Any() ?? false;
        // 
        //     if (descriptorHasSockets == false && dtoHasChildren == true)
        //     {
        //         throw new ArgumentException($"Children and Socket mismatch. DescriptorHasSockets = {descriptorHasSockets}, BlueprintPieceDtoHasChldren = {dtoHasChildren}, {nameof(BlueprintPieceDto)}.{nameof(BlueprintPieceDto.Key)} = {dto.Key}");
        //     }
        // 
        //     if (descriptorHasSockets == true && dtoHasChildren == false)
        //     {
        //         return Array.Empty<BlueprintPiece[]>();
        //     }
        // 
        //     if (descriptorHasSockets == false)
        //     {
        //         return Array.Empty<BlueprintPiece[]>();
        //     }
        // 
        // 
        // 
        //     BlueprintPiece[][] children = new BlueprintPiece[sockets.Items.count][];
        // 
        //     for (int i = 0; i < sockets.Items.count; i++)
        //     {
        //         if (dto.Children.Length <= i || dto.Children[i] is null)
        //         {
        //             children[i] = Array.Empty<BlueprintPiece>();
        //             continue;
        //         }
        // 
        //         children[i] = dto.Children[i]!
        //             .Select(x => new BlueprintPiece(x, pieces))
        //             .ToArray();
        //     }
        // 
        //     return children;
        // }

        // public EntityId Spawn(VhId treeId, Id<ITeam> teamId, IEntityService entities)
        // {
        //     VhId vhid = HashBuilder<BlueprintPieceDto, VhId, Id<BlueprintDto>>.Instance.Calculate(treeId, this.Blueprint.Id);
        // 
        //     return _pieceSpawner.Spawn(treeId, vhid, teamId, entities);
        // }
    }
}
