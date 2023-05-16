using Guppy.Attributes;
using Guppy.Common.DependencyInjection;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Domain.Entities.ShipParts.Systems;

namespace VoidHuntersRevived.Domain.Entities.ShipParts.Loaders
{
    [AutoLoad]
    internal sealed class SystemLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureCollection(manager =>
            {
                manager.AddScoped<RigidSystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<DestroyShipPartEntitySystem>()
                    .AddInterfaceAliases();
            });
        }
    }
}
