using Autofac;
using Guppy.Attributes;
using Guppy.Common.Autofac;
using Guppy.Loaders;
using LiteNetLib;
using System.Runtime.CompilerServices;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Constants;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Messages;
using VoidHuntersRevived.Domain.Simulations.Predictive;
using VoidHuntersRevived.Domain.Simulations.Services;

[assembly: InternalsVisibleTo("VoidHuntersRevived.Domain.Client")]

namespace VoidHuntersRevived.Domain.Simulations.Loaders
{
    [AutoLoad]
    public sealed class SimulationLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            services.RegisterType<SimulationService>().As<ISimulationService>().InstancePerMatchingLifetimeScope(LifetimeScopeTags.GuppyScope);

            this.ConfigureLockstep(services);
            this.ConfigurePredictive(services);
        }

        private void ConfigureLockstep(ContainerBuilder services)
        {
            services.RegisterType<LockstepSimulation_Server>().AsImplementedInterfaces().InstancePerLifetimeScope();
            services.RegisterType<LockstepSimulation_Client>().AsImplementedInterfaces().InstancePerLifetimeScope();
            services.RegisterType<TickBuffer>().InstancePerMatchingLifetimeScope(LifetimeScopeTags.GuppyScope);

            services.AddNetMessageType<Tick>(DeliveryMethod.ReliableUnordered, 0);
            services.AddNetMessageType<TickHistoryStart>(DeliveryMethod.ReliableOrdered, 0);
            services.AddNetMessageType<TickHistoryItem>(DeliveryMethod.ReliableOrdered, 0);
            services.AddNetMessageType<TickHistoryEnd>(DeliveryMethod.ReliableOrdered, 0);
        }

        private void ConfigurePredictive(ContainerBuilder services)
        {
            services.RegisterType<PredictiveSimulation>().AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}
