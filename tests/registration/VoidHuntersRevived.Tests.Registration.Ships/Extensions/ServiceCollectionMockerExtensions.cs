using Serilog;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Serialization.Components;
using VoidHuntersRevived.Domain.Ships.Common.Services;
using VoidHuntersRevived.Domain.Ships.Engines;
using VoidHuntersRevived.Domain.Ships.Services;
using VoidHuntersRevived.Domain.Teams.Common.Services;
using VoidHuntersRevived.Tests.Common;

namespace VoidHuntersRevived.Tests.Registration.Ships.Extensions
{
    public static class ServiceCollectionMockerExtensions
    {
        public static void RegisterShipsServices(this ServiceCollectionMocker services)
        {
            services.RegisterFactory<TacticalService>(provider => new(
                provider.Get<IEntityQueryService>()));

            services.RegisterFactory<TractorBeamEmitterService>(provider => new(
                provider.Get<ISpace>(),
                provider.Get<IEntityQueryService>(),
                provider.Get<IEntitySpawnService>(),
                provider.Get<IEntitySerializationService>(),
                provider.Get<INodeService>(),
                provider.Get<ITreeService>(),
                provider.Get<ISocketService>(),
                provider.Get<ITeamService>(),
                provider.Get<ILogger>()));

            services.RegisterFactory<TractorBeamEmitterInputEngine>(provider => new(
                provider.Get<ITractorBeamEmitterService>(),
                provider.Get<IEntityQueryService>(),
                provider.Get<ILogger>()));

            services.RegisterFactory<TractorBeamEmitterUpdateEngine>(provider => new(
                provider.Get<IEntityQueryService>(),
                provider.Get<ISpace>(),
                provider.Get<ILogger>(),
                provider.Get<ITractorBeamEmitterService>(),
                provider.Get<ISocketService>()));

            services.RegisterFactory<TacticalEngine>(provider => new(
                provider.Get<IEntityQueryService>()));

            services.RegisterFactory<HelmComponentSerializer>(provider => new());
            services.RegisterFactory<TacticalComponentSerializer>(provider => new());
            services.RegisterFactory<TractorableComponentSerializer>(provider => new());
            services.RegisterFactory<TractorBeamEmitterComponentSerializer>(provider => new());
            services.RegisterFactory<UserIdComponentSerializer>(provider => new());
        }
    }
}
