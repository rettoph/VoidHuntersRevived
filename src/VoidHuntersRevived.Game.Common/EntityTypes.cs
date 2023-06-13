using System.Runtime.InteropServices;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Game.Common
{
    public static class EntityTypes
    {
        internal static readonly EntityType Ship = new EntityType(VoidHuntersRevivedGame.NameSpace, nameof(Ship));
        public static readonly EntityType UserShip = new EntityType(VoidHuntersRevivedGame.NameSpace, nameof(Ship));
    }
}
