using Guppy.Engine;
using System.Reflection;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Game.Core;

namespace VoidHuntersRevived.Presentation.Core
{
    public static class VoidHuntersContextBuilder
    {
        private static readonly Assembly[] GameLibraries = [
            typeof(VhId).Assembly
        ];

        public static GuppyContext Build()
        {
            return new GuppyContext(VoidHuntersRevivedGame.Company, VoidHuntersRevivedGame.Name, null);
        }
        public static GuppyContext Build(string suffix)
        {
            return new GuppyContext(VoidHuntersRevivedGame.Company, VoidHuntersRevivedGame.Name + "." + suffix, null);
        }
    }
}
