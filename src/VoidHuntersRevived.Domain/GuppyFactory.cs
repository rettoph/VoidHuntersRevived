using Guppy;
using Guppy.Attributes;
using Guppy.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Domain.Entities.Loaders;
using VoidHuntersRevived.Domain.Loaders;
using VoidHuntersRevived.Domain.Simulations.Loaders;

namespace VoidHuntersRevived.Domain
{
    [AutoLoad]
    internal sealed class GuppyFactory : IGuppyFactory
    {
        public void Build(GuppyEngine guppy)
        {
            guppy.AddLoader(new EntityLoader(), 0);
            guppy.AddLoader(new SimulationLoader(), 0);
        }
    }
}
