using Guppy.Core.Common.Builders;
using Guppy.Core.Common.Extensions;
using Guppy.Game.Common.Extensions;
using VoidHuntersRevived.Game.Server.Components.Scene;
using VoidHuntersRevived.Game.Server.Guppy;

namespace VoidHuntersRevived.Game.Server.Extensions
{
    public static class IGuppyRootBuilderExtensions
    {
        public static IGuppyRootBuilder RegisterGameServerServices(this IGuppyRootBuilder builder)
        {
            return builder.EnsureRegisteredOnce(nameof(RegisterGameServerServices), builder =>
            {
                builder.RegisterSceneFilter<ServerGameScene>(builder =>
                {
                    builder.RegisterSceneSystem<ConfigureSimulationsSystem>();
                    builder.RegisterSceneSystem<ServerPeerSystem>();
                });
            });
        }
    }
}