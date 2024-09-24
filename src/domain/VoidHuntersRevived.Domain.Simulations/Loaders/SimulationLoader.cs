using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Engine.Common.Loaders;
using LiteNetLib;
using System.Runtime.CompilerServices;
using VoidHuntersRevived.Domain.Simulations.Common.Factories;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Factories;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Messages;
using VoidHuntersRevived.Domain.Simulations.Services;

[assembly: InternalsVisibleTo("VoidHuntersRevived.Domain.Client")]

namespace VoidHuntersRevived.Domain.Simulations.Loaders
{
    [AutoLoad]
    public sealed class SimulationLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder builder)
        {
            builder.RegisterType<StrategiesFactory>().As<IStrategiesFactory>().InstancePerLifetimeScope();
            builder.RegisterType<SimulationService>().As<ISimulationService>().InstancePerLifetimeScope();
            builder.RegisterType<EngineService>().As<IEngineService>().InstancePerLifetimeScope();

            this.ConfigureLockstep(builder);
            this.ConfigurePredictive(builder);
        }

        private void ConfigureLockstep(ContainerBuilder services)
        {
            services.RegisterType<TickBuffer>().InstancePerLifetimeScope();

            services.AddNetMessageType<Tick>(DeliveryMethod.ReliableUnordered, 0);
            services.AddNetMessageType<TickHistoryStart>(DeliveryMethod.ReliableOrdered, 0);
            services.AddNetMessageType<TickHistoryItem>(DeliveryMethod.ReliableOrdered, 0);
            services.AddNetMessageType<TickHistoryEnd>(DeliveryMethod.ReliableOrdered, 0);
        }

        private void ConfigurePredictive(ContainerBuilder services)
        {
        }
    }
}
