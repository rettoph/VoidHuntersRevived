using Autofac;
using Guppy.Attributes;
using Guppy.Files;
using Guppy.Game.MonoGame.Utilities.Cameras;
using Guppy.Loaders;
using Guppy.Resources.Configuration;
using Guppy.Resources.Extensions.Autofac;

namespace VoidHuntersRevived.Game.Client.Loaders
{
    [AutoLoad]
    internal sealed class MainLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            services.RegisterType<Camera2D>().As<Camera>().AsSelf().SingleInstance();

            services.RegisterResourcePack(new ResourcePackConfiguration()
            {
                EntryDirectory = DirectoryLocation.CurrentDirectory(VoidHuntersPack.Directory)
            });
        }
    }
}
