using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Game.Entities
{
    public static class EntityTypes
    {
        public static readonly EntityType Ship = new EntityType(nameof(Ship));
        public static readonly EntityType ShipPart = new EntityType(nameof(ShipPart));
    }
}
