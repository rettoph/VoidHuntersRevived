using Autofac;
using Guppy.Core.Files.Common;
using Guppy.Core.Resources.Configuration;
using Guppy.Core.Resources.Extensions.Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Engine.Common.Loaders;

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
