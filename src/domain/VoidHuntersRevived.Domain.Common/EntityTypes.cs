using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Pieces.Common.Descriptors;
using VoidHuntersRevived.Domain.Ships.Common.Descriptors;

namespace VoidHuntersRevived.Domain.Common
{
    public static class EntityTypes
    {
        public static readonly EntityType<ChainDescriptor> Chain = new(nameof(Chain));
        public static readonly EntityType<UserShipDescriptor> UserShip = new(nameof(UserShip));

        public static class Pieces
        {
            public static EntityType<HullDescriptor> HullSquare = new($"Piece.Hull.Square");
        }
    }
}
