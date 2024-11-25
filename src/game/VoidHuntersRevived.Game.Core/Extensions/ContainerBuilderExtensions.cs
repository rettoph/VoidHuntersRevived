using Autofac;
using Guppy.Core.Common.Extensions.Autofac;
using Guppy.Core.Files.Common;
using Guppy.Core.Resources.Common.Configuration;
using Guppy.Core.Resources.Common.Extensions.Autofac;
using VoidHuntersRevived.Domain.Graphics.Common.Extensions.Autofac;
using VoidHuntersRevived.Domain.Pieces.Common.Graphics.Vertices;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;
using VoidHuntersRevived.Game.Core.Components.Scene;
using VoidHuntersRevived.Game.Core.Engines;
using VoidHuntersRevived.Game.Core.Graphics.Effects;

namespace VoidHuntersRevived.Game.Core.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder RegisterGameCoreServices(this ContainerBuilder builder)
        {
            return builder.EnsureRegisteredOnce(nameof(RegisterGameCoreServices), builder =>
            {
                builder.RegisterType<SimulationFrameComponent>().AsImplementedInterfaces().InstancePerLifetimeScope();

                builder.RegisterEngine<SimulationEngine>();
                builder.RegisterEngine<UserEngine>();

                builder.RegisterType<ShaderAntiAliasingEffect>().SingleInstance();
                builder.RegisterType<VisibleEffect>().AsImplementedInterfaces().AsSelf().SingleInstance();

                builder.RegisterPrimitiveType<VertexVisible, VertexStaticVisible, VisibleEffect>("PrimitiveType.Visible");

                builder.RegisterResourcePack(new ResourcePackConfiguration()
                {
                    EntryDirectory = DirectoryLocation.CurrentDirectory(VoidHuntersPack.Directory)
                });
            });
        }
    }
}
