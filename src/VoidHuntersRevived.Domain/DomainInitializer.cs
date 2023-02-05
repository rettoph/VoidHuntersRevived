using Guppy;
using Guppy.Attributes;
using Guppy.Common.Providers;
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
    internal sealed class DomainInitializer : IGuppyInitializer
    {
        public void Initialize(GuppyEngine engine)
        {
            engine.Assemblies.Load(typeof(EntityLoader).Assembly);
            engine.Assemblies.Load(typeof(SimulationLoader).Assembly);
        }
    }
}
