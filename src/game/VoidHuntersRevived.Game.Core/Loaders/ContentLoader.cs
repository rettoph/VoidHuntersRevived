using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Engine.Common.Loaders;
using VoidHuntersRevived.Domain.Graphics.Common.Extensions.Autofac;
using VoidHuntersRevived.Domain.Pieces.Common.Graphics.Vertices;
using VoidHuntersRevived.Game.Core.Graphics.Effects;

namespace VoidHuntersRevived.Game.Core.Loaders
{
    [AutoLoad]
    internal sealed class ContentLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder builder)
        {
            builder.RegisterType<ShaderAntiAliasingEffect>().SingleInstance();
            builder.RegisterType<VisibleEffect>().AsImplementedInterfaces().AsSelf().SingleInstance();

            builder.RegisterPrimitiveType<VertexVisible, VertexStaticVisible, VisibleEffect>("PrimitiveType.Visible");
        }
    }
}
