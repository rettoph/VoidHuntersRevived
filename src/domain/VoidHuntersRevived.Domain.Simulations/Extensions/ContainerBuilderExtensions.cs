using Autofac;
using Guppy.Core.Common.Extensions.Autofac;
using Guppy.Core.Network.Common.Enums;
using Guppy.Core.StateMachine.Common.Providers;
using Guppy.Game.Graphics.Common.Extensions;
using LiteNetLib;
using VoidHuntersRevived.Domain.Serialization.NetSerializers;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Common.Predictive;
using VoidHuntersRevived.Domain.Simulations.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Engines.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Messages;
using VoidHuntersRevived.Domain.Simulations.Predictive;
using VoidHuntersRevived.Domain.Simulations.Serialization.NetSerializers;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Domain.Simulations.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder RegisterDomainSimulationServices(this ContainerBuilder builder)
        {
            return builder.EnsureRegisteredOnce(nameof(RegisterDomainSimulationServices), builder =>
                                                                                                                   {
                                                                                                                       builder.RegisterType<SimulationService>().As<ISimulationService>().InstancePerLifetimeScope();

                                                                                                                       builder.RegisterType<TickBuffer>().InstancePerLifetimeScope();

                                                                                                                       builder.RegisterNetMessageType<Tick>(DeliveryMethod.ReliableUnordered, 0);
                                                                                                                       builder.RegisterNetMessageType<TickHistoryStart>(DeliveryMethod.ReliableOrdered, 0);
                                                                                                                       builder.RegisterNetMessageType<TickHistoryItem>(DeliveryMethod.ReliableOrdered, 0);
                                                                                                                       builder.RegisterNetMessageType<TickHistoryEnd>(DeliveryMethod.ReliableOrdered, 0);

                                                                                                                       builder.RegisterEngine<LockstepClient_TickEngine>();
                                                                                                                       builder.RegisterEngine<LockstepServer_TickEngine>();
                                                                                                                       builder.RegisterEngine<LockstepServer_UserEngine>();

                                                                                                                       builder.RegisterNetMessageType<EventDto>(DeliveryMethod.ReliableUnordered, 0);

                                                                                                                       builder.RegisterNetSerializer<TickHistoryEndNetSerializer>();
                                                                                                                       builder.RegisterNetSerializer<TickHistoryItemNetSerializer>();
                                                                                                                       builder.RegisterNetSerializer<TickHistoryStartNetSerializer>();
                                                                                                                       builder.RegisterNetSerializer<TickNetSerializer>();
                                                                                                                       builder.RegisterNetSerializer<EventDtoNetSerializer>();
                                                                                                                       builder.RegisterNetSerializer<Simulation_Begin_NetSerializer>();
                                                                                                                       builder.RegisterNetSerializer<UserJoinedNetSerializer>();

                                                                                                                       builder.RegisterType<StrategyTypeStateProvider>().As<IStateProvider>().InstancePerLifetimeScope();

                                                                                                                       builder.RegisterPeerTypeFilter<IClientEngine>(PeerTypeEnum.Client);
                                                                                                                       builder.RegisterPeerTypeFilter<IServerEngine>(PeerTypeEnum.Server);
                                                                                                                       builder.RegisterGraphicsEnabledFilter<IGraphicsEngine>(true);
                                                                                                                       builder.RegisterStrategyFilter<IPredictiveSynchronizationEngine, IPredictiveStrategy>();

                                                                                                                       const string StrategyLoggerContext = nameof(Strategy);
                                                                                                                       builder.RegisterLoggerContext<Strategy>(StrategyLoggerContext);
                                                                                                                       builder.RegisterLoggerContext<PredictiveStrategy>(StrategyLoggerContext);
                                                                                                                       builder.RegisterLoggerContext<LockstepStrategy_Client>(StrategyLoggerContext);
                                                                                                                       builder.RegisterLoggerContext<LockstepStrategy_Server>(StrategyLoggerContext);
                                                                                                                   });
        }
    }
}