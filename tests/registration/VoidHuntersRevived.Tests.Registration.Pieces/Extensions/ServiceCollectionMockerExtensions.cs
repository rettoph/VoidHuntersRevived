using Serilog;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Engines;
using VoidHuntersRevived.Domain.Pieces.Serialization.Components;
using VoidHuntersRevived.Domain.Pieces.Services;
using VoidHuntersRevived.Domain.Ships.Common.Services;
using VoidHuntersRevived.Tests.Common;

namespace VoidHuntersRevived.Tests.Registration.Pieces.Extensions
{
    public static class ServiceCollectionMockerExtensions
    {
        public static void RegisterPiecesServices(this ServiceCollectionMocker services)
        {
            services.RegisterFactory<NodeService>(provider => new(
                provider.Get<IEntityQueryService>()));

            services.RegisterFactory<TreeService>(provider => new(
                provider.Get<IEntityQueryService>(),
                provider.Get<IEntitySpawnService>(),
                provider.Get<IBlueprintService>()));

            services.RegisterFactory<SocketService>(provider => new(
                provider.Get<IEntityQueryService>(),
                provider.Get<IEntitySpawnService>(),
                provider.Get<IEntitySerializationService>(),
                provider.Get<ITreeService>(),
                provider.Get<ILogger>()));

            services.RegisterFactory<CouplingEngine>(provider => new(
                provider.Get<ISocketService>(),
                provider.Get<ILogger>()));

            services.RegisterFactory<NodeEngine>(provider => new(
                provider.Get<IEntityQueryService>(),
                provider.Get<IEntitySpawnService>(),
                provider.Get<ISocketService>(),
                provider.Get<ILogger>()));

            services.RegisterFactory<RigidEngine>(provider => new(
                provider.Get<ISpace>(),
                provider.Get<IEntityQueryService>(),
                provider.Get<ILogger>()));

            services.RegisterFactory<ThrustableEngine>(provider => new(
                provider.Get<IEntityQueryService>(),
                provider.Get<ISpace>()));

            services.RegisterFactory<SocketIdsEngine>(provider => new(
                provider.Get<IEntityQueryService>(),
                provider.Get<IEntitySpawnService>(),
                provider.Get<ISocketService>(),
                provider.Get<ILogger>()));

            services.RegisterFactory<TractorableEngine>(provider => new(
                provider.Get<ITractorBeamEmitterService>(),
                provider.Get<ITacticalService>(),
                provider.Get<IEntityQueryService>(),
                provider.Get<ILogger>()));

            services.RegisterFactory<TreeEngine>(provider => new(
                provider.Get<IEntityQueryService>(),
                provider.Get<IEntitySpawnService>(),
                provider.Get<ILogger>()));

            services.RegisterFactory<CouplingComponentSerializer>(provider => new(
                provider.Get<IEntityQueryService>()));
            services.RegisterFactory<NodeComponentSerializer>(provider => new(
                provider.Get<IEntityQueryService>()));
            services.RegisterFactory<PlugComponentSerializer>(provider => new());
            services.RegisterFactory<SocketIdsComponentSerializer>(provider => new(
                provider.Get<IEntityQueryService>(),
                provider.Get<ISocketService>()));
            services.RegisterFactory<TreeComponentSerializer>(provider => new());
        }
    }
}
