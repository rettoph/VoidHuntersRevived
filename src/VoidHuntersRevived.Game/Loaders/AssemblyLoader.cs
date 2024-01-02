using Guppy.Attributes;
using Guppy.Common.Providers;
using Guppy.Loaders;
using VoidHuntersRevived.Domain.Loaders;
using VoidHuntersRevived.Game.Common.Loaders;
using VoidHuntersRevived.Game.Ships.Loaders;

namespace VoidHuntersRevived.Game.Loaders
{
    [AutoLoad]
    internal sealed class AssemblyLoader : IAssemblyLoader
    {
        public void ConfigureAssemblies(IAssemblyProvider assemblies)
        {
            assemblies.Load(typeof(DomainLoader).Assembly);
            assemblies.Load(typeof(EntityTypeLoader).Assembly);
            assemblies.Load(typeof(ShipLoader).Assembly);
        }
    }
}
