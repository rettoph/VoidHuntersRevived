using Serilog;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Physics.Engines;
using VoidHuntersRevived.Domain.Physics.Serialization.Components;
using VoidHuntersRevived.Tests.Common;

namespace VoidHuntersRevived.Tests.Registration.Physics.Extensions
{
    public static class ServiceCollectionMockerExtensions
    {
        public static void RegisterPhysicsServices(this ServiceCollectionMocker services)
        {
            services.RegisterFactory<Space>(provider => new(
                provider.Get<ILogger>(),
                new World(AetherVector2.Zero)));

            services.RegisterFactory<BodyAwakeEngine>(provider => new(
                provider.Get<IEntityQueryService>(),
                provider.Get<ILogger>(),
                provider.Get<ISpace>()));

            services.RegisterFactory<BodyLocationEngine>(provider => new(
                provider.Get<IEntityQueryService>(),
                provider.Get<ISpace>()));

            services.RegisterFactory<BodyPhysicsBubbleEngine>(provider => new(
                provider.Get<IEntityQueryService>(),
                provider.Get<ISpace>()));

            services.RegisterFactory<SpaceEngine>(provider => new(
                provider.Get<ISpace>()));

            services.RegisterFactory<AwakeComponentSerializer>(provider => new());
            services.RegisterFactory<CollisionComponentSerializer>(provider => new());
            services.RegisterFactory<EnabledComponentSerializer>(provider => new());
            services.RegisterFactory<LocationComponentSerializer>(provider => new());
            services.RegisterFactory<PhysicsBubbleComponentSerializer>(provider => new());
        }
    }
}
