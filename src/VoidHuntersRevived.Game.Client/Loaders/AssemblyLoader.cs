using Guppy.Attributes;
using Guppy.Common.Providers;
using Guppy.Loaders;
using VoidHuntersRevived.Domain.Client.Loaders;

namespace VoidHuntersRevived.Game.Client.Loaders
{
    [AutoLoad]
    internal sealed class AssemblyLoader : IAssemblyLoader
    {
        public void ConfigureAssemblies(IAssemblyProvider assemblies)
        {
            assemblies.Load(typeof(ClientLoader).Assembly);
        }
    }
}
