using Guppy.Attributes;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Domain.Client.Systems;
using Guppy.Common.DependencyInjection;

namespace VoidHuntersRevived.Domain.Client.Loaders
{
    [AutoLoad]
    internal sealed class SystemLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureCollection(manager =>
            {
                manager.AddScoped<CameraSystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<DrawableShipPartSystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<DrawTacticalSystem>()
                    .AddInterfaceAliases();

                // manager.AddScoped<DebugAetherSystem>()
                //     .AddInterfaceAliases();

                manager.AddScoped<InputSystem>()
                    .AddInterfaceAliases();
            });
        }
    }
}
