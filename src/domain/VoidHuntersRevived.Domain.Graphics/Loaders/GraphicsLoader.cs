using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Engine.Common.Loaders;
using VoidHuntersRevived.Domain.Graphics.Services;

namespace VoidHuntersRevived.Domain.Graphics.Loaders
{
    [AutoLoad]
    public sealed class GraphicsLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder builder)
        {
            builder.RegisterType<PrimitiveService>().AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}
