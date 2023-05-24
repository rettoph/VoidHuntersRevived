using Guppy.Attributes;
using Guppy.Common.DependencyInjection;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Domain.Entities.Services;
using VoidHuntersRevived.Domain.Entities.Systems;
using VoidHuntersRevived.Domain.Simulations.Systems;

namespace VoidHuntersRevived.Domain.Entities.Loaders
{
    [AutoLoad]
    internal sealed class SystemLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureCollection(manager =>
            {
                manager.AddScoped<VoltWorldSystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<Systems.EntitySystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<UserShipSystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<HelmSystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<TacticalSystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<TractorBeamEmitterSystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<HelmBodySystem>()
                    .AddInterfaceAliases();
            });
        }
    }
}
