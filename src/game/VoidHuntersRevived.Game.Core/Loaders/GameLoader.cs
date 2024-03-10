using Autofac;
using Guppy.Attributes;
using Guppy.Files;
using Guppy.Loaders;
using Guppy.Resources.Configuration;
using Guppy.Resources.Extensions.Autofac;

namespace VoidHuntersRevived.Game.Core.Loaders
{
    [AutoLoad]
    public class GameLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            services.RegisterResourcePack(new ResourcePackConfiguration()
            {
                EntryDirectory = DirectoryLocation.CurrentDirectory(VoidHuntersPack.Directory)
            });
        }
    }
}
