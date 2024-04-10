using Guppy.Attributes;
using Guppy.Common.Services;
using Guppy.Loaders;
using VoidHuntersRevived.Game.Core.Loaders;

namespace VoidHuntersRevived.Presentation.Core.Loaders
{
    [AutoLoad]
    internal sealed class GameAssemblyLoader : IAssemblyLoader
    {
        public void ConfigureAssemblies(IAssemblyService assemblies)
        {
            assemblies.Load(typeof(GameLoader).Assembly);
        }
    }
}
