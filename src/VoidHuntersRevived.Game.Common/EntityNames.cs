using System.Runtime.InteropServices;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Game.Common
{
    public static class EntityNames
    {
        public static readonly EntityName Chain = new EntityName(VoidHuntersRevivedGame.NameSpace, nameof(Chain));
        public static readonly EntityName UserShip = new EntityName(VoidHuntersRevivedGame.NameSpace, nameof(UserShip));
    }
}
