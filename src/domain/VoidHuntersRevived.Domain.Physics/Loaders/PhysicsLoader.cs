using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Resources.Common.Extensions.Autofac;
using Guppy.Engine.Common.Loaders;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Domain.Physics.Engines;
using VoidHuntersRevived.Domain.Physics.ResourceTypes;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;

namespace VoidHuntersRevived.Domain.Physics.Loaders
{
    [AutoLoad]
    public sealed class PhysicsLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            services.Register<AetherWorld>(c => new AetherWorld(AetherVector2.Zero)).InstancePerLifetimeScope();

            services.RegisterResourceType<BodyTemplateResourceType>();

            services.RegisterEngine<BodyAwakeEngine>();
            services.RegisterEngine<BodyCollisionEngine>();
            services.RegisterEngine<BodyLocationEngine>();
            services.RegisterEngine<BodyLocationPredictiveSynchronizationEngine>();
            services.RegisterEngine<BodyPhysicsBubbleEngine>();
            services.RegisterEngine<SpaceEngine>();
            services.RegisterEngine<Space>();
        }
    }
}
