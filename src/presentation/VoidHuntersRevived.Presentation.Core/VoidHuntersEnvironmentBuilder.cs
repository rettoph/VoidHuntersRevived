using Guppy.Core.Common;
using Guppy.Core.Common.Builders;
using VoidHuntersRevived.Game.Core;

namespace VoidHuntersRevived.Presentation.Core
{
    public static class VoidHuntersEnvironmentBuilder
    {
        public static readonly IEnumerable<IEnvironmentVariable> ClientEnvironment = new EnvironmentVariablesBuilder(VoidHuntersRevivedGame.Company, $"{VoidHuntersRevivedGame.Project}").Build();
        public static readonly IEnumerable<IEnvironmentVariable> ServerEnvironment = new EnvironmentVariablesBuilder(VoidHuntersRevivedGame.Company, $"{VoidHuntersRevivedGame.Project}.Server").Build();
    }
}