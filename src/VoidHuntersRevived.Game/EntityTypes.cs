using System.Runtime.InteropServices;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Game.Common;
using VoidHuntersRevived.Game.Descriptors;

namespace VoidHuntersRevived.Game
{
    public static class EntityTypes
    {
        public static readonly EntityType<ChainDescriptor> Chain = new(VoidHuntersRevivedGame.NameSpace, nameof(Chain));
        public static readonly EntityType<ShipDescriptor> UserShip = new(VoidHuntersRevivedGame.NameSpace, nameof(UserShip));
    }
}
