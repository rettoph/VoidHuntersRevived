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

        public static readonly GuppyContext ClientContext = new(VoidHuntersRevivedGame.Company, $"{VoidHuntersRevivedGame.Name}", GameLibraries);
        public static readonly GuppyContext ServerContext = new(VoidHuntersRevivedGame.Company, $"{VoidHuntersRevivedGame.Name}.Server", GameLibraries);
    }
}
