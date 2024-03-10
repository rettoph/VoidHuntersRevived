using Autofac;
using Guppy.Attributes;
using Guppy.Loaders;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Common.Physics;

namespace VoidHuntersRevived.Domain.Physics.Loaders
{
    [AutoLoad]
    public sealed class PhysicsLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            services.Register<AetherWorld>(c => new AetherWorld(AetherVector2.Zero)).InstancePerLifetimeScope();
            services.RegisterType<Space>().AsSelf().As<ISpace>().InstancePerLifetimeScope();
        }
    }
}
