using Autofac;
using VoidHuntersRevived.Domain.Graphics.Common.Extensions.Autofac;
using VoidHuntersRevived.Domain.Pieces.Common.Graphics.Vertices;
using VoidHuntersRevived.Game.Core.Graphics.Effects;

namespace VoidHuntersRevived.Game.Core.Modules
{
    internal sealed class ContentModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ShaderAntiAliasingEffect>().SingleInstance();
            builder.RegisterType<VisibleEffect>().AsImplementedInterfaces().AsSelf().SingleInstance();

            builder.RegisterPrimitiveType<VertexVisible, VertexStaticVisible, VisibleEffect>("PrimitiveType.Visible");
        }
    }
}
