using Guppy;
using Guppy.Attributes;
using Guppy.Common.Providers;
using Guppy.Configurations;
using Guppy.Loaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Domain.Entities.Loaders;
using VoidHuntersRevived.Domain.Simulations.Loaders;

namespace VoidHuntersRevived.Domain
{
    [AutoLoad]
    internal sealed class DomainConfigurator : IGuppyConfigurator
    {
        public void Configure(GuppyConfiguration configuration)
        {
            configuration.Assemblies.Load(typeof(EntityLoader).Assembly);
            configuration.Assemblies.Load(typeof(SimulationLoader).Assembly);
        }
    }
}
