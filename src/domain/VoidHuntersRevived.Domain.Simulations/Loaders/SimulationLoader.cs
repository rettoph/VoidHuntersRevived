using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Core.StateMachine.Common.Providers;
using Guppy.Engine.Common.Loaders;
using LiteNetLib;
using System.Runtime.CompilerServices;
using VoidHuntersRevived.Domain.Serialization.NetSerializers;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Engines.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Messages;
using VoidHuntersRevived.Domain.Simulations.Serialization.NetSerializers;
using VoidHuntersRevived.Domain.Simulations.Services;

[assembly: InternalsVisibleTo("VoidHuntersRevived.Domain.Client")]

namespace VoidHuntersRevived.Domain.Simulations.Loaders
{
    [AutoLoad]
    public sealed class SimulationLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder builder)
        {
            builder.RegisterType<SimulationService>().As<ISimulationService>().InstancePerLifetimeScope();
            builder.RegisterType<EngineService>().As<IEngineService>().InstancePerLifetimeScope();

            builder.RegisterType<StrategyTypeStateProvider>().As<IStateProvider>().InstancePerLifetimeScope();

            this.ConfigureLockstep(builder);
            this.ConfigurePredictive(builder);
        }

        private void ConfigureLockstep(ContainerBuilder services)
        {
            services.RegisterType<TickBuffer>().InstancePerLifetimeScope();

            services.RegisterNetMessageType<Tick>(DeliveryMethod.ReliableUnordered, 0);
            services.RegisterNetMessageType<TickHistoryStart>(DeliveryMethod.ReliableOrdered, 0);
            services.RegisterNetMessageType<TickHistoryItem>(DeliveryMethod.ReliableOrdered, 0);
            services.RegisterNetMessageType<TickHistoryEnd>(DeliveryMethod.ReliableOrdered, 0);

            services.RegisterEngine<LockstepClient_TickEngine>();
            services.RegisterEngine<LockstepServer_TickEngine>();
            services.RegisterEngine<LockstepServer_UserEngine>();

            services.RegisterNetSerializer<TickHistoryEndNetSerializer>();
            services.RegisterNetSerializer<TickHistoryItemNetSerializer>();
            services.RegisterNetSerializer<TickHistoryStartNetSerializer>();
            services.RegisterNetSerializer<TickNetSerializer>();
        }

        private void ConfigurePredictive(ContainerBuilder services)
        {
        }
    }
}
