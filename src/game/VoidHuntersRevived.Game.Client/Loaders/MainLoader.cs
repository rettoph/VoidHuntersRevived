using Autofac;
using Guppy.Core.Files.Common;
using Guppy.Core.Resources.Common.Configuration;
using Guppy.Core.Resources.Common.Extensions.Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Engine.Common.Loaders;
using Guppy.Game.MonoGame.Utilities.Cameras;
using VoidHuntersRevived.Game.Client.Graphics.Effects;

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

            services.RegisterType<ShaderAntiAliasingEffect>().InstancePerDependency();
            services.RegisterType<VisibleAccumEffect>().InstancePerDependency();
            services.RegisterType<VisibleFinalEffect>().InstancePerDependency();
        }
    }
}
