using Autofac;
using Guppy.Core.Common.Builders;
using Guppy.Core.Common.Extensions;
using Guppy.Core.Network.Common.Enums;
using Guppy.Core.Network.Common.Extensions;
using Guppy.Game.Common.Extensions;
using LiteNetLib;
using VoidHuntersRevived.Domain.Common;
using VoidHuntersRevived.Domain.Serialization.NetSerializers;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;
using VoidHuntersRevived.Domain.Simulations.Messages;
using VoidHuntersRevived.Domain.Simulations.Serialization.NetSerializers;
using VoidHuntersRevived.Domain.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations.Systems;

namespace VoidHuntersRevived.Domain.Simulations.Extensions
{
    public static class IGuppyRootBuilderExtensions
    {
        public static IGuppyRootBuilder RegisterDomainSimulationServices(this IGuppyRootBuilder builder)
        {
            return builder.EnsureRegisteredOnce(nameof(RegisterDomainSimulationServices), builder =>
            {
                builder.RegisterNetMessageType<Tick, TickNetSerializer>(DeliveryMethod.ReliableUnordered, 0);
                builder.RegisterNetMessageType<TickHistoryStart, TickHistoryStartNetSerializer>(DeliveryMethod.ReliableOrdered, 0);
                builder.RegisterNetMessageType<TickHistoryItem, TickHistoryItemNetSerializer>(DeliveryMethod.ReliableOrdered, 0);
                builder.RegisterNetMessageType<TickHistoryEnd, TickHistoryEndNetSerializer>(DeliveryMethod.ReliableOrdered, 0);
                builder.RegisterNetMessageType<EnqueuedStepInput, EnqueuedStepInputNetSerializer>(DeliveryMethod.ReliableUnordered, 0);

                builder.RegisterNetSerializer<Simulation_Begin_NetSerializer>();
                builder.RegisterNetSerializer<UserJoinedNetSerializer>();

                builder.RegisterSceneFilter<IVoidHuntersGameScene>(builder =>
                {
                    builder.RegisterType<SimulationService>().AsSelf().As<ISimulationService>().InstancePerLifetimeScope();
                });

                builder.RegisterSceneFilter<IStrategy>(builder =>
                {
                    builder.RegisterSceneSystem<StepServiceUpdateSystem>();

                    builder.RegisterSceneFilter<IPredictiveStrategy>(builder =>
                    {
                        builder.RegisterSceneSystem<PredictiveStepEventCleanSystem>();

                        builder.RegisterType<PredictiveStepEventService>().AsSelf().As<IStepEventService>().InstancePerLifetimeScope();
                        builder.RegisterType<PredictiveStepService>().AsSelf().As<IStepService>().InstancePerLifetimeScope();
                    });

                    builder.RegisterSceneFilter<ILockstepStrategy>(builder =>
                    {
                        builder.RegisterType<DefaultLockstepTickService>().As<ITickService>().InstancePerLifetimeScope();
                        builder.RegisterType<DefaultLockstepStepEventService>().AsSelf().As<IStepEventService>().InstancePerLifetimeScope();
                        builder.RegisterType<DefaultLockstepStepService>().AsSelf().As<IStepService>().InstancePerLifetimeScope();

                        builder.RegisterSceneSystem<LockstepStrategyTickPublishInputsSystem>();

                        builder.RegisterPeerTypeFilter(PeerTypeEnum.Client, builder =>
                        {
                            builder.RegisterType<ClientLinkedListLockstepTickService>().AsSelf().As<ITickService>().InstancePerLifetimeScope();
                            builder.RegisterType<ClientLockstepStepEventService>().AsSelf().As<IStepEventService>().InstancePerLifetimeScope();

                            builder.RegisterSceneSystem<LockstepStrategyClientSystem>();
                        });

                        builder.RegisterPeerTypeFilter(PeerTypeEnum.Server, builder =>
                        {
                            builder.RegisterSceneSystem<LockstepStrategyServerSystem>();
                        });
                    });
                });
            });
        }
    }
}