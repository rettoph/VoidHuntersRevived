using Guppy.Attributes;
using Guppy.Common.DependencyInjection;
using Guppy.Loaders;
using Guppy.MonoGame;
using Guppy.MonoGame.Utilities.Cameras;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Domain.Client.Debuggers;
using VoidHuntersRevived.Domain.Client.Systems;

namespace VoidHuntersRevived.Domain.Client.Loaders
{
    [AutoLoad]
    internal sealed class MainLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGuppy<ClientGameGuppy>();

            services.ConfigureCollection(manager =>
            {
                manager.AddScoped<CameraSystem>()
                    .AddInterfaceAliases();

                manager.AddSingleton<Camera2D>()
                    .AddAlias<Camera>();

                services.AddService<LockstepStateDebugger>()
                    .SetLifetime(ServiceLifetime.Scoped)
                    .AddAlias<IDebugger>();

                services.AddService<AetherDebugger>()
                    .SetLifetime(ServiceLifetime.Scoped)
                    .AddAlias<IDebugger>();
            });
        }
    }
}
