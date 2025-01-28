using System.Reflection;
using Guppy.Core.Common;
using Guppy.Core.Common.Utilities;
using Guppy.Engine.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Game.Core;

namespace VoidHuntersRevived.Presentation.Core
{
    public static class VoidHuntersEnvironmentBuilder
    {
        private static readonly Assembly[] _gameLibraries = [
            typeof(VhId).Assembly,
            typeof(IGuppyEngine).Assembly
        ];

        public static readonly GuppyEnvironment ClientEnvironment = new GuppyEnvironmentBuilder(VoidHuntersRevivedGame.Company, $"{VoidHuntersRevivedGame.Project}", _gameLibraries).Build();
        public static readonly GuppyEnvironment ServerEnvironment = new GuppyEnvironmentBuilder(VoidHuntersRevivedGame.Company, $"{VoidHuntersRevivedGame.Project}.Server", _gameLibraries).Build();
    }
}