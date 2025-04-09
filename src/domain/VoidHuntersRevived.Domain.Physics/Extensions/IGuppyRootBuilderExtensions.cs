using Guppy.Core.Common.Builders;
using Guppy.Core.Common.Extensions;
using Guppy.Core.Assets.Common.Extensions;
using Guppy.Core.Serialization.Common.Extensions;
using Guppy.Game.Common.Extensions;
using Svelto.ECS;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Domain.Entities.Common.Exceptions;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Physics.AssetTypes;
using VoidHuntersRevived.Domain.Physics.Serialization.Components;
using VoidHuntersRevived.Domain.Physics.Serialization.Json;
using VoidHuntersRevived.Domain.Physics.Serialization.Json.Converters;
using VoidHuntersRevived.Domain.Physics.Systems;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;

namespace VoidHuntersRevived.Domain.Physics.Extensions
{
    public static class IGuppyRootBuilderExtensions
    {
        public static IGuppyRootBuilder RegisterDomainPhysicsServices(this IGuppyRootBuilder builder)
        {
            return builder.EnsureRegisteredOnce(nameof(RegisterDomainPhysicsServices), builder =>
            {
                builder.RegisterAssetType<BodyTemplateAssetType>();

                builder.RegisterJsonConverter<PolygonConverter>();
                builder.RegisterJsonConverter<BodyTemplateConverter>();
                builder.RegisterJsonConverter<RigidJsonConverter>();

                builder.RegisterPolymorphicJsonType<Rigid, IEntityComponent>(nameof(Rigid));

                builder.RegisterComponentSerializer<AwakeComponentSerializer>();
                builder.RegisterComponentSerializer<CollisionComponentSerializer>();
                builder.RegisterComponentSerializer<EnabledComponentSerializer>();
                builder.RegisterComponentSerializer<PhysicsBubbleComponentSerializer>();
                builder.RegisterComponentSerializer<BodyComponentSerializer>();
                builder.RegisterComponentSerializer<FixtureComponentSerializer>();

                builder.RegisterSceneFilter<IStrategy>(builder =>
                {
                    builder.Register<AetherWorld>(c => new AetherWorld(AetherVector2.Zero)).InstancePerLifetimeScope();
                    builder.RegisterType<Space>().As<ISpace>().InstancePerLifetimeScope();

                    builder.RegisterSceneSystem<BodyAwakeSystem>();
                    builder.RegisterSceneSystem<BodyCollisionSystem>();
                    builder.RegisterSceneSystem<BodyLocationSystem>();
                    builder.RegisterSceneSystem<BodyPhysicsBubbleSystem>();
                    builder.RegisterSceneSystem<SpaceSystem>();
                    builder.RegisterSceneSystem<RigidFixtureSystem>();

                    builder.RegisterSceneFilter<IPredictiveStrategy>(builder =>
                    {
                        builder.RegisterSceneSystem<BodyLocationPredictiveSynchronizationSystem>();
                    });
                });
            });
        }
    }
}