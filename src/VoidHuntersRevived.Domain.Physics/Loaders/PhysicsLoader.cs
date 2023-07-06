using Autofac;
using Guppy.Attributes;
using Guppy.Loaders;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Physics.Factories;
using VoidHuntersRevived.Domain.Physics.Factories;

namespace VoidHuntersRevived.Domain.Physics.Loaders
{
    [AutoLoad]
    public sealed class PhysicsLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            services.RegisterType<BodyFactory>().As<IBodyFactory>().InstancePerLifetimeScope();
            services.RegisterType<Space>().As<ISpace>().InstancePerLifetimeScope();
        }
    }
}
