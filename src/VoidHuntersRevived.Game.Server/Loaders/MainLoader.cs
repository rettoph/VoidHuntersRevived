using Autofac;
using Guppy.Attributes;
using Guppy.Loaders;

namespace VoidHuntersRevived.Game.Server.Loaders
{
    [AutoLoad]
    internal sealed class MainLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
        }
    }
}
