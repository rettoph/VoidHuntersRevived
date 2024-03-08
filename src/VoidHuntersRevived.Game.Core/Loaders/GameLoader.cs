using Autofac;
using Guppy.Attributes;
using Guppy.Files.Enums;
using Guppy.Loaders;
using Guppy.Resources.Extensions.Autofac;

namespace VoidHuntersRevived.Game.Core.Loaders
{
    [AutoLoad]
    public class GameLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            services.ConfigureResourcePacks((scope, packs) =>
            {
                packs.Add(FileType.CurrentDirectory, Path.Combine(VoidHuntersPack.Directory, "pack.json"));
            });
        }
    }
}
