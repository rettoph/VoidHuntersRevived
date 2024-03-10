using Guppy.Attributes;
using Guppy.Common.Providers;
using Guppy.Loaders;
using VoidHuntersRevived.Game.Core.Loaders;

namespace VoidHuntersRevived.Presentation.Core.Loaders
{
    [AutoLoad]
    internal sealed class GameAssemblyLoader : IAssemblyLoader
    {
        public void ConfigureAssemblies(IAssemblyProvider assemblies)
        {
            assemblies.Load(typeof(GameLoader).Assembly);
        }
    }
}
