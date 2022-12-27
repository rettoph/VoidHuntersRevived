using Guppy.Attributes;
using Guppy.Loaders;
using Guppy.MonoGame;
using Guppy.MonoGame.Utilities.Cameras;
using Microsoft.Extensions.DependencyInjection;
using MonoGame.Extended.Entities.Systems;
using VoidHuntersRevived.Client.Library.Debuggers;
using VoidHuntersRevived.Client.Library.Systems;
using Guppy.Common.DependencyInjection;
using Guppy.Common;

namespace VoidHuntersRevived.Client.Library.Loaders
{
    [AutoLoad(0)]
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
                    .AddInterfaceAliases();

                manager.AddService<LocalCurrentUserSystem>()
                    .SetLifetime(ServiceLifetime.Scoped)
                    .AddAlias<ISubscriber>()
                    .AddAlias<ISystem>(x =>
                    {
                        x.SetOrder(int.MinValue);
                    });

                manager.GetService<WorldDebugger>()
                    .SetLifetime(ServiceLifetime.Scoped)
                    .AddInterfaceAliases();
            });
        }
    }
}
