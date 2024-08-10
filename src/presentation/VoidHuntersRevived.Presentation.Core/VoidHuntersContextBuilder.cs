using Guppy.Engine;
using System.Reflection;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Teams.Common.EntityTypes;
using VoidHuntersRevived.Game.Core;

namespace VoidHuntersRevived.Presentation.Core
{
    public static class VoidHuntersContextBuilder
    {
        private static readonly Assembly[] GameLibraries = [
            typeof(VhId).Assembly,
            typeof(TeamEntityType).Assembly
        ];

        public static readonly GuppyContext ClientContext = new GuppyContext(VoidHuntersRevivedGame.Company, $"{VoidHuntersRevivedGame.Name}", GameLibraries);
        public static readonly GuppyContext ServerContext = new GuppyContext(VoidHuntersRevivedGame.Company, $"{VoidHuntersRevivedGame.Name}.Server", GameLibraries);
    }
}
