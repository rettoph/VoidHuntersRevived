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
using VoidHuntersRevived.Domain.ECS.Loaders;
using VoidHuntersRevived.Domain.Entities.Loaders;
using VoidHuntersRevived.Domain.Entities.ShipParts.Loaders;
using VoidHuntersRevived.Domain.Physics.Loaders;
using VoidHuntersRevived.Domain.Simulations.Loaders;

namespace VoidHuntersRevived.Domain
{
    [AutoLoad]
    internal sealed class DomainConfigurator : IGuppyConfigurator
    {
        public void Configure(GuppyConfiguration configuration)
        {
            // configuration.Assemblies.Load(typeof(EntityLoader).Assembly);
            configuration.Assemblies.Load(typeof(SimulationLoader).Assembly);
            // configuration.Assemblies.Load(typeof(ShipPartEntityLoader).Assembly);
            configuration.Assemblies.Load(typeof(PhysicsLoader).Assembly);
            configuration.Assemblies.Load(typeof(ECSLoader).Assembly);
        }
    }
}
