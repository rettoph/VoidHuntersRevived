using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Services;
using Guppy.Engine.Common.Loaders;
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
