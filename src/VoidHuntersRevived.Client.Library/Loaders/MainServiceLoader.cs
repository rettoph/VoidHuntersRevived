using Guppy.Attributes;
using Guppy.Loaders;
using Guppy.MonoGame;
using Guppy.MonoGame.Utilities.Cameras;
using Microsoft.Extensions.DependencyInjection;
using MonoGame.Extended.Entities.Systems;
using VoidHuntersRevived.Client.Library.Debuggers;
using VoidHuntersRevived.Client.Library.Systems;
using Guppy.Common.DependencyInjection;

namespace VoidHuntersRevived.Client.Library.Loaders
{
    [AutoLoad]
    internal sealed class MainServiceLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGuppy<ClientMainGuppy>();
            services.AddGuppy<ClientGameGuppy>();

            services.AddScoped<Camera2D>();

            services.ConfigureCollection(manager =>
            {
                manager.GetService<AetherDebugSystem>()
                    .SetLifetime(ServiceLifetime.Scoped)
                    .AddAlias<ISystem>();

                manager.GetService<WorldDebugger>()
                    .SetLifetime(ServiceLifetime.Scoped)
                    .AddAlias<IDebugger>();
            });
        }
    }
}
