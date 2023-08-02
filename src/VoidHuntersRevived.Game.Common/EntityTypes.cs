using System.Runtime.InteropServices;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Ships.Descriptors;
using VoidHuntersRevived.Game.Common;
using VoidHuntersRevived.Game.Common.Descriptors;

namespace VoidHuntersRevived.Game.Common
{
    public static class EntityTypes
    {
        public static readonly EntityType<ChainDescriptor> Chain = new(nameof(Chain));
        public static readonly EntityType<ShipDescriptor> UserShip = new(nameof(UserShip));
    }
}
