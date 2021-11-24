using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Components.Entities.ShipParts;
using VoidHuntersRevived.Client.Library.Services;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Client.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class ShipPartServiceLoader : IServiceLoader
    {
        public void RegisterServices(AssemblyHelper assemblyHelper, GuppyServiceCollection services)
        {
            services.RegisterTypeFactory<ShipPartRenderService>(p => new ShipPartRenderService());
            services.RegisterScoped<ShipPartRenderService>();

            services.RegisterTypeFactory<ShipPartDrawComponent>(p => new ShipPartDrawComponent());
            services.RegisterTransient<ShipPartDrawComponent>();
            services.RegisterComponent<ShipPartDrawComponent, ShipPart>();
        }

        public void ConfigureProvider(GuppyServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
