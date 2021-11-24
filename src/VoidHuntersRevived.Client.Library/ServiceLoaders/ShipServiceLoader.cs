using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Components.Entities.ShipParts;
using VoidHuntersRevived.Client.Library.Components.Entities.Ships;
using VoidHuntersRevived.Client.Library.Services;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.Ships;

namespace VoidHuntersRevived.Client.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class ShipServiceLoader : IServiceLoader
    {
        public void RegisterServices(AssemblyHelper assemblyHelper, GuppyServiceCollection services)
        {
            services.RegisterTypeFactory<ShipTractorBeamDrawComponent>(p => new ShipTractorBeamDrawComponent());
            services.RegisterTransient<ShipTractorBeamDrawComponent>();
            services.RegisterComponent<ShipTractorBeamDrawComponent, Ship>();
        }

        public void ConfigureProvider(GuppyServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
