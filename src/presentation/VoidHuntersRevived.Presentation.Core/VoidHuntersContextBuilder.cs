using System.Reflection;
using Guppy.Core.Common;
using Guppy.Engine.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Game.Core;

namespace VoidHuntersRevived.Presentation.Core
{
    public static class VoidHuntersContextBuilder
    {
        private static readonly Assembly[] _gameLibraries = [
            typeof(VhId).Assembly,
            typeof(IGuppyEngine).Assembly
        ];

        public static readonly GuppyContext ClientContext = new(VoidHuntersRevivedGame.Company, $"{VoidHuntersRevivedGame.Name}", _gameLibraries);
        public static readonly GuppyContext ServerContext = new(VoidHuntersRevivedGame.Company, $"{VoidHuntersRevivedGame.Name}.Server", _gameLibraries);
    }
}