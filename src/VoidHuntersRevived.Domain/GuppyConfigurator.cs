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
using VoidHuntersRevived.Domain.Simulations.Loaders;
using VoidHuntersRevived.Domain.Physics.Loaders;
using VoidHuntersRevived.Domain.Loaders;
using VoidHuntersRevived.Domain.Entities.Loaders;
using VoidHuntersRevived.Domain.Pieces.Loaders;

namespace VoidHuntersRevived.Domain
{
    [AutoLoad]
    internal sealed class GuppyConfigurator : IGuppyConfigurator
    {
        public void Configure(GuppyConfiguration configuration)
        {
            configuration.Assemblies.Load(typeof(SimulationLoader).Assembly);
            configuration.Assemblies.Load(typeof(PhysicsLoader).Assembly);
            configuration.Assemblies.Load(typeof(EntityLoader).Assembly);
            configuration.Assemblies.Load(typeof(PieceLoader).Assembly);
        }
    }
}
