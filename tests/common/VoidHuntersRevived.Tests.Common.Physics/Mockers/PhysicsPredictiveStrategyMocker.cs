using VoidHuntersRevived.Domain.Physics.Serialization.Components;
using VoidHuntersRevived.Domain.Physics.Systems;
using VoidHuntersRevived.Tests.Common.Physics.Builders;
using VoidHuntersRevived.Tests.Common.Physics.Interfaces;
using VoidHuntersRevived.Tests.Common.Teams.Mockers;

namespace VoidHuntersRevived.Tests.Common.Physics.Mockers
{
    public class PhysicsPredictiveStrategyMocker : TeamPredictiveStrategyMocker, IPhysicsStrategyMocker
    {
        public AetherWorldBuilder AetherWorldBuilder { get; }
        public SpaceBuilder SpaceBuilder { get; }

        public PhysicsPredictiveStrategyMocker()
        {
            this.AetherWorldBuilder = new AetherWorldBuilder();
            this.SpaceBuilder = new SpaceBuilder()
            {
                AetherWorldBuilder = this.AetherWorldBuilder,
                LoggerMocker = this.LoggerMocker
            };

            this.ComponentSerializerServiceBuilder.ComponentSerializers.AddRange([
                () => new AwakeComponentSerializer(),
                () => new CollisionComponentSerializer(),
                () => new EnabledComponentSerializer(),
                () => new PhysicsBubbleComponentSerializer(),
                () => new BodyComponentSerializer(
                    logger: this.LoggerMocker.Object),
                () => new FixtureComponentSerializer(
                    entityQueryService: this.EntityQueryServiceBuilder.Object,
                    logger: this.LoggerMocker.Object)
            ]);

            this.SystemFactories.AddRange([
                x => new BodyAwakeSystem(
                    entityQueryService: this.EntityQueryServiceBuilder.Object,
                    logger: this.LoggerMocker.Object,
                    space: this.SpaceBuilder.Object),
                x => new BodyCollisionSystem(
                    entityQueryService: this.EntityQueryServiceBuilder.Object,
                    space: this.SpaceBuilder.Object),
                x => new BodyLocationSystem(
                    entityQueryService: this.EntityQueryServiceBuilder.Object,
                    space: this.SpaceBuilder.Object),
                x => new BodyPhysicsBubbleSystem(
                    entityQueryService: this.EntityQueryServiceBuilder.Object,
                    space: this.SpaceBuilder.Object),
                x => new SpaceSystem(
                    space: this.SpaceBuilder.Object),
                x => new RigidFixtureSystem(
                    space: this.SpaceBuilder.Object,
                    entityQueryService: this.EntityQueryServiceBuilder.Object,
                    logger: this.LoggerMocker.Object),
                x => new BodyLocationPredictiveSynchronizationSystem(
                    space: this.SpaceBuilder.Object,
                    logger: this.LoggerServiceMocker.Object.GetLogger<BodyLocationPredictiveSynchronizationSystem>())
            ]);
        }
    }
}
