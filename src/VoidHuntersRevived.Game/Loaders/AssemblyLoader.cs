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
using VoidHuntersRevived.Domain.Physics.Loaders;
using VoidHuntersRevived.Domain.Loaders;
using VoidHuntersRevived.Domain.Pieces.Loaders;
using VoidHuntersRevived.Game.Ships.Loaders;
using VoidHuntersRevived.Game.Common.Loaders;

namespace VoidHuntersRevived.Game.Loaders
{
    [AutoLoad]
    internal sealed class AssemblyLoader : IAssemblyLoader
    {
        public void ConfigureAssemblies(IAssemblyProvider assemblies)
        {
            assemblies.Load(typeof(DomainLoader).Assembly);
            assemblies.Load(typeof(EntityTypeLoader).Assembly);
            assemblies.Load(typeof(ShipsServiceLoader).Assembly);
        }
    }
}
