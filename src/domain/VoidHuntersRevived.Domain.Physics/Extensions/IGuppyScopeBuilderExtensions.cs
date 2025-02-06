using Guppy.Core.Common;
using Guppy.Core.Common.Extensions;
using Guppy.Core.Resources.Common.Extensions;
using Guppy.Core.Serialization.Common.Extensions;
using Guppy.Game.Common.Extensions;
using Svelto.ECS;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Domain.Entities.Common.Exceptions;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Physics.ResourceTypes;
using VoidHuntersRevived.Domain.Physics.Serialization.Components;
using VoidHuntersRevived.Domain.Physics.Serialization.Json;
using VoidHuntersRevived.Domain.Physics.Serialization.Json.Converters;
using VoidHuntersRevived.Domain.Physics.Systems;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;
using VoidHuntersRevived.Domain.Simulations.Common.Predictive;

namespace VoidHuntersRevived.Domain.Physics.Extensions
{
    public static class IGuppyScopeBuilderExtensions
    {
        public static IGuppyScopeBuilder RegisterDomainPhysicsServices(this IGuppyScopeBuilder builder)
        {
            builder.RegisterResourceType<BodyTemplateResourceType>();

            builder.RegisterJsonConverter<PolygonConverter>();
            builder.RegisterJsonConverter<BodyTemplateConverter>();
            builder.RegisterJsonConverter<RigidJsonConverter>();

            return builder.EnsureRegisteredOnce(nameof(RegisterDomainPhysicsServices), builder =>
            {
                builder.RegisterPolymorphicJsonType<Rigid, IEntityComponent>(nameof(Rigid));

                builder.RegisterSceneFilter<IStrategy>(builder =>
                {
                    builder.Register<AetherWorld>(c => new AetherWorld(AetherVector2.Zero)).InstancePerLifetimeScope();

                    builder.RegisterEngine<BodyAwakeSystem>();
                    builder.RegisterEngine<BodyCollisionSystem>();
                    builder.RegisterEngine<BodyLocationSystem>();
                    builder.RegisterEngine<BodyPhysicsBubbleSystem>();
                    builder.RegisterEngine<SpaceSystem>();
                    builder.RegisterEngine<RigidFixtureSystem>();
                    builder.RegisterEngine<Space>();

                    builder.RegisterComponentSerializer<AwakeComponentSerializer>();
                    builder.RegisterComponentSerializer<CollisionComponentSerializer>();
                    builder.RegisterComponentSerializer<EnabledComponentSerializer>();
                    builder.RegisterComponentSerializer<PhysicsBubbleComponentSerializer>();
                    builder.RegisterComponentSerializer<BodyComponentSerializer>();
                    builder.RegisterComponentSerializer<FixtureComponentSerializer>();

                    builder.RegisterSceneFilter<IPredictiveStrategy>(builder =>
                    {
                        builder.RegisterEngine<BodyLocationPredictiveSynchronizationSystem>();
                    });
                });
            });
        }
    }
}