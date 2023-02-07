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
    internal sealed class DomainInitializer : IGuppyBuilder
    {
        public void Initialize(GuppyConfiguration boot)
        {
            boot.Assemblies.Load(typeof(EntityLoader).Assembly);
            boot.Assemblies.Load(typeof(SimulationLoader).Assembly);
        }
    }
}
