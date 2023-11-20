using Guppy.Attributes;
using Guppy.Common.Providers;
using Guppy.Loaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Domain.Client.Loaders;
using VoidHuntersRevived.Domain.Entities.Loaders;
using VoidHuntersRevived.Domain.Physics.Loaders;
using VoidHuntersRevived.Domain.Pieces.Loaders;
using VoidHuntersRevived.Domain.Simulations.Loaders;

namespace VoidHuntersRevived.Domain.Loaders
{
    [AutoLoad]
    internal class AssemblyLoader : IAssemblyLoader
    {
        public void ConfigureAssemblies(IAssemblyProvider assemblies)
        {
            assemblies.Load(typeof(SimulationLoader).Assembly);
            assemblies.Load(typeof(PhysicsLoader).Assembly);
            assemblies.Load(typeof(EntityLoader).Assembly);
            assemblies.Load(typeof(PieceLoader).Assembly);
            assemblies.Load(typeof(ClientLoader).Assembly);
        }
    }
}
