using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components.Instance;
using VoidHuntersRevived.Common.Pieces.Services;

namespace VoidHuntersRevived.Domain.Pieces
{
    internal sealed class BlueprintPiece : IBlueprintPiece
    {
        private readonly string _pieceTypeKey;
        private readonly Lazy<IPieceTypeService> _pieceTypes;
        private PieceType? _pieceType;
        private bool _initialized;

        public PieceType PieceType => _pieceType ??= this.InitializePieceType();
        public IBlueprintPiece[][] Children { get; }

        public BlueprintPiece(string pieceTypeKey, IBlueprintPiece[][] children, Lazy<IPieceTypeService> pieceTypes)
        {
            _pieceTypeKey = pieceTypeKey;
            _pieceTypes = pieceTypes;

            this.Children = children;
        }

        private PieceType InitializePieceType()
        {
            if (!_pieceTypes.Value.TryGetByKey(_pieceTypeKey, out PieceType? pieceType))
            {
                throw new ArgumentException($"Unknown {nameof(Common.Pieces.PieceType)}.{nameof(Common.Pieces.PieceType.Key)} - {_pieceTypeKey}");
            }

            if (this.Children!.Length > 0)
            {
                Sockets<Location> sockets = pieceType.InstanceComponents.Values.OfType<Sockets<Location>>().First();

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
