using Guppy.Attributes;
using Guppy.Common.Providers;
using Guppy.Initializers;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Entities.Initializers
{
    [AutoLoad]
    internal sealed class ShipPartInitializer : IGuppyInitializer
    {
        public void Initialize(IAssemblyProvider assemblies, IServiceCollection services, IEnumerable<IGuppyLoader> loaders)
        {
            var configurationLoaders = assemblies.GetTypes<IShipPartConfigurationLoader>().WithAttribute<AutoLoadAttribute>(true);

            foreach(var configurationLoader in configurationLoaders)
            {
                services.AddSingleton(typeof(IShipPartConfigurationLoader), configurationLoader);
            }
        }
    }
}
