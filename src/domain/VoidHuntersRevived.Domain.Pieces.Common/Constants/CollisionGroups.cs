using VoidHuntersRevived.Domain.Physics.Common;

namespace VoidHuntersRevived.Domain.Pieces.Common.Constants
{
    public static class CollisionGroups
    {
        public static readonly CollisionGroup FreeFloatingCategories = CollisionGroup.Create(nameof(FreeFloatingCategories), CollisionCategories.FreeFloating);
        public static readonly CollisionGroup FreeFloatingCollidesWith = CollisionGroup.Create(nameof(FreeFloatingCollidesWith));

        public static readonly CollisionGroup ShipCategories = CollisionGroup.Create(nameof(ShipCategories), CollisionCategories.Ship);
        public static readonly CollisionGroup ShipCollidesWith = CollisionGroup.Create(nameof(ShipCollidesWith), CollisionCategories.Ship);
    }
}
