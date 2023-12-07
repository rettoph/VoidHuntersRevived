using Autofac;
using Guppy.Attributes;
using Guppy.Loaders;
using Guppy.Game.MonoGame.Utilities.Cameras;
using Guppy.Extensions.Autofac;
using Serilog;
using Guppy.Common.Autofac;
using Guppy.Files.Providers;
using Guppy.Files.Enums;
using Guppy.Files.Helpers;
using Guppy.Game.Extensions.Serilog;
using Guppy.Game.Common;

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
