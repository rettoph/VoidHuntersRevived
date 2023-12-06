using Autofac;
using Guppy.Attributes;
using Guppy.Loaders;
using Guppy.Game.MonoGame.Utilities.Cameras;

namespace VoidHuntersRevived.Game.Client.Loaders
{
    [AutoLoad]
    internal sealed class MainLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            services.RegisterType<Camera2D>().As<Camera>().AsSelf().SingleInstance();
        }
    }
}
