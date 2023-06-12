using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Game.Common
{
    public static class EntityTypes
    {
        public static readonly EntityType Pilot = new EntityType(nameof(Pilot));
        public static readonly EntityType Ship = new EntityType(nameof(Ship));
        public static readonly EntityType ShipPart = new EntityType(nameof(ShipPart));
    }
}
