using Autofac;
using Guppy.Core.Common.Extensions.Autofac;
using Guppy.Core.Resources.Common.Extensions.Autofac;
using System.Text.Json.Serialization;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Domain.Entities.Common.Exceptions;
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
                builder.RegisterEngine<Space>();

                builder.RegisterType<PolygonConverter>().As<JsonConverter>().SingleInstance();
                builder.RegisterType<BodyTemplateConverter>().As<JsonConverter>().SingleInstance();

                builder.RegisterComponentSerializer<AwakeComponentSerializer>();
                builder.RegisterComponentSerializer<CollisionComponentSerializer>();
                builder.RegisterComponentSerializer<EnabledComponentSerializer>();
                builder.RegisterComponentSerializer<LocationComponentSerializer>();
                builder.RegisterComponentSerializer<PhysicsBubbleComponentSerializer>();
            });
        }
    }
}
