using System.Runtime.InteropServices;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Game.Common
{
    public static class EntityTypes
    {
        public static readonly EntityType Tree = new EntityType(VoidHuntersRevivedGame.NameSpace, nameof(Tree));
        public static readonly EntityType Ship = new EntityType(VoidHuntersRevivedGame.NameSpace, nameof(Ship));
    }
}
