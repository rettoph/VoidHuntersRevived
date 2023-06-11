using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.DependencyInjection;
using Guppy.Common.DependencyInjection.Interfaces;
using Guppy.Loaders;
using Guppy.Network;
using LiteNetLib;
using Microsoft.Extensions.DependencyInjection;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Constants;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Messages;
using VoidHuntersRevived.Domain.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations.Systems;

namespace VoidHuntersRevived.Domain.Simulations.Loaders
{
    [AutoLoad]
    public sealed class SimulationLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSetting<Fix64>(Settings.StepInterval, (Fix64)20, false);
            services.AddSetting<int>(Settings.StepsPerTick, 3, false);

            services.AddScoped<ISimulationService, SimulationService>();

            services.ConfigureCollection(manager =>
            {
                this.ConfigureLockstep(services, manager);
                this.ConfigurePredictive(services, manager);
            });
        }

        private void ConfigureLockstep(IServiceCollection services, IServiceCollectionManager manager)
        {
            manager.AddScoped<LockstepSimulation_Server>()
                .AddInterfaceAliases();

            manager.AddScoped<LockstepSimulation_Client>()
                .AddInterfaceAliases();

            services.AddNetMessageType<Tick>(DeliveryMethod.ReliableUnordered, 0);
            services.AddNetMessageType<EventDto>(DeliveryMethod.ReliableUnordered, 0);
            services.AddNetMessageType<TickHistoryStart>(DeliveryMethod.ReliableOrdered, 0);
            services.AddNetMessageType<TickHistoryItem>(DeliveryMethod.ReliableOrdered, 0);
            services.AddNetMessageType<TickHistoryEnd>(DeliveryMethod.ReliableOrdered, 0);
        }

        private void ConfigurePredictive(IServiceCollection services, IServiceCollectionManager manager)
        {
        }
    }
}
