using System.Reflection;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Game.Core
{
    public class VoidHuntersRevivedGame
    {
        public const string Company = "rettoph";
        public const string Project = "VoidHuntersRevived";
        public static HashSet<Assembly> Libraries = [
            typeof(VhId).Assembly
        ];

        public static readonly VhId NameSpace = NameSpace<VoidHuntersRevivedGame>.Instance;

    }
}