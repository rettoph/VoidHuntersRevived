using Guppy.Attributes;
using Guppy.Common.DependencyInjection;
using Guppy.Loaders;
using Guppy.MonoGame.Utilities.Cameras;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Client.Systems;

namespace VoidHuntersRevived.Library.Client.Loaders
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

                manager.AddScoped<AetherDebugSystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<Camera2D>()
                    .AddAlias<Camera>();
            });
        }
    }
}
