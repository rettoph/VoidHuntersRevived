using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Files.Common;
using Guppy.Core.Resources.Common.Configuration;
using Guppy.Core.Resources.Common.Extensions.Autofac;
using Guppy.Engine.Common.Loaders;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;
using VoidHuntersRevived.Game.Core.Components.Scene;
using VoidHuntersRevived.Game.Core.Engines;

namespace VoidHuntersRevived.Game.Core.Loaders
{
    [AutoLoad]
    public class GameLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder builder)
        {
            builder.RegisterType<SimulationFrameComponent>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterEngine<SimulationEngine>();
            builder.RegisterEngine<UserEngine>();

            builder.RegisterResourcePack(new ResourcePackConfiguration()
            {
                EntryDirectory = DirectoryLocation.CurrentDirectory(VoidHuntersPack.Directory)
            });
        }
    }
}
