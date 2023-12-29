using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Pieces.Descriptors;
using VoidHuntersRevived.Common.Ships.Descriptors;
using VoidHuntersRevived.Game.Common.Descriptors;

namespace VoidHuntersRevived.Game.Common
{
    public static class EntityTypes
    {
        public static readonly EntityType<ChainDescriptor> Chain = new(nameof(Chain));
        public static readonly EntityType<UserShipDescriptor> UserShip = new(nameof(UserShip));

        public static class Pieces
        {
            public static EntityType<HullDescriptor> HullSquare = new(nameof(HullSquare));
            public static EntityType<HullDescriptor> HullTriangle = new(nameof(HullTriangle));

            public static EntityType<ThrusterDescriptor> Thruster = new(nameof(Thruster));
        }
    }
}
