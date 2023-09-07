using Autofac;
using Guppy.Attributes;
using Guppy.Loaders;
using VoidHuntersRevived.Common.Physics;

namespace VoidHuntersRevived.Domain.Physics.Loaders
{
    [AutoLoad]
    public sealed class PhysicsLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            services.RegisterType<Space>().AsSelf().As<ISpace>().InstancePerLifetimeScope();
        }
    }
}
