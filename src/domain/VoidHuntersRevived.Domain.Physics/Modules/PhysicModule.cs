using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Resources.Common.Extensions.Autofac;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Domain.Physics.Engines;
using VoidHuntersRevived.Domain.Physics.ResourceTypes;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;

namespace VoidHuntersRevived.Domain.Physics.Modules
{
    [AutoLoad]
    public sealed class PhysicModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register<AetherWorld>(c => new AetherWorld(AetherVector2.Zero)).InstancePerLifetimeScope();

            builder.RegisterResourceType<BodyTemplateResourceType>();

            builder.RegisterEngine<BodyAwakeEngine>();
            builder.RegisterEngine<BodyCollisionEngine>();
            builder.RegisterEngine<BodyLocationEngine>();
            builder.RegisterEngine<BodyLocationPredictiveSynchronizationEngine>();
            builder.RegisterEngine<BodyPhysicsBubbleEngine>();
            builder.RegisterEngine<SpaceEngine>();
            builder.RegisterEngine<Space>();
        }
    }
}
