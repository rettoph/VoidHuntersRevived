using Autofac;
using Guppy.Core.Common.Extensions.Autofac;
using Guppy.Core.Resources.Common.Extensions.Autofac;
using Guppy.Core.Serialization.Common.Extensions;
using Svelto.ECS;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Domain.Entities.Common.Exceptions;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Physics.Engines;
using VoidHuntersRevived.Domain.Physics.ResourceTypes;
using VoidHuntersRevived.Domain.Physics.Serialization.Components;
using VoidHuntersRevived.Domain.Physics.Serialization.Json;
using VoidHuntersRevived.Domain.Physics.Serialization.Json.Converters;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;

namespace VoidHuntersRevived.Domain.Physics.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder RegisterDomainPhysicsServices(this ContainerBuilder builder)
        {
            return builder.EnsureRegisteredOnce(nameof(RegisterDomainPhysicsServices), builder =>
            {
                builder.Register<AetherWorld>(c => new AetherWorld(AetherVector2.Zero)).InstancePerLifetimeScope();

                builder.RegisterResourceType<BodyTemplateResourceType>();

                builder.RegisterEngine<BodyAwakeEngine>();
                builder.RegisterEngine<BodyCollisionEngine>();
                builder.RegisterEngine<BodyLocationEngine>();
                builder.RegisterEngine<BodyLocationPredictiveSynchronizationEngine>();
                builder.RegisterEngine<BodyPhysicsBubbleEngine>();
                builder.RegisterEngine<SpaceEngine>();
                builder.RegisterEngine<RigidFixtureEngine>();
                builder.RegisterEngine<Space>();

                builder.RegisterJsonConverter<PolygonConverter>();
                builder.RegisterJsonConverter<BodyTemplateConverter>();
                builder.RegisterJsonConverter<RigidJsonConverter>();

                builder.RegisterComponentSerializer<AwakeComponentSerializer>();
                builder.RegisterComponentSerializer<CollisionComponentSerializer>();
                builder.RegisterComponentSerializer<EnabledComponentSerializer>();
                builder.RegisterComponentSerializer<PhysicsBubbleComponentSerializer>();
                builder.RegisterComponentSerializer<BodyComponentSerializer>();
                builder.RegisterComponentSerializer<FixtureComponentSerializer>();

                builder.RegisterPolymorphicJsonType<Rigid, IEntityComponent>(nameof(Rigid));
            });
        }
    }
}