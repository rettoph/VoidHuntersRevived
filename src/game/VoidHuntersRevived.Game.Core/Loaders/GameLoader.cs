using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Files.Common;
using Guppy.Core.Resources.Common.Configuration;
using Guppy.Core.Resources.Common.Extensions.Autofac;
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
