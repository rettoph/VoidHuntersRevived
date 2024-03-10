using VoidHuntersRevived.Domain.Physics.Common;

namespace VoidHuntersRevived.Domain.Pieces.Common.Constants
{
    public static class CollisionCategories
    {
        public static readonly CollisionCategory FreeFloating = CollisionCategory.Create(nameof(FreeFloating));
        public static readonly CollisionCategory Ship = CollisionCategory.Create(nameof(Ship));
    }
}
