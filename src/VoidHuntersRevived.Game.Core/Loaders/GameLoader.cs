using Autofac;
using Guppy.Attributes;
using Guppy.Loaders;

namespace VoidHuntersRevived.Game.Core.Loaders
{
    [AutoLoad]
    public class GameLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            //
        }
    }
}
