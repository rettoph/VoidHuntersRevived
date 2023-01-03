using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.DependencyInjection;
using Guppy.Common.DependencyInjection.Interfaces;
using Guppy.Loaders;
using LiteNetLib;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Services;
using VoidHuntersRevived.Library.Common;
using VoidHuntersRevived.Library.Constants;
using VoidHuntersRevived.Library.Simulations.Lockstep;
using VoidHuntersRevived.Library.Simulations.Lockstep.Factories;
using VoidHuntersRevived.Library.Simulations.Lockstep.Providers;
using VoidHuntersRevived.Library.Simulations.Lockstep.Services;
using VoidHuntersRevived.Library.Simulations.Predictive;
using VoidHuntersRevived.Library.Simulations.Services;
using VoidHuntersRevived.Library.Simulations.Systems;
using Lockstep = VoidHuntersRevived.Library.Simulations.Lockstep.Systems;

namespace VoidHuntersRevived.Library.Loaders
{
    [AutoLoad]
    internal sealed class SimulationLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSetting<TimeSpan>(SettingConstants.StepInterval, TimeSpan.FromMilliseconds(20), false);
            services.AddSetting<int>(SettingConstants.StepsPerTick, 3, false);

            services.AddScoped<ISimulationService, SimulationService>()
                    .AddScoped<ISimulatedService, SimulatedService>();

            services.ConfigureCollection(manager =>
            {
                manager.AddScoped<AetherSystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<UserPilotSystem>()
                    .AddInterfaceAliases();

                this.ConfigureLockstep(services, manager);
                this.ConfigurePredictive(services, manager);
            });
        }

        private void ConfigureLockstep(IServiceCollection services, IServiceCollectionManager manager)
        {
            manager.AddScoped<LockstepSimulation>()
                .AddInterfaceAliases();

            manager.AddScoped<ServerLockstepEventPublishingService>()
                .AddAlias<ILockstepEventPublishingService>();

            manager.AddScoped<ClientLockstepEventPublishingService>()
                .AddAlias<ILockstepEventPublishingService>();

            manager.AddScoped<State>();

            manager.AddScoped<ClientTickFactory>()
                .AddAlias<ITickFactory>();

            manager.AddScoped<ServerTickFactory>()
                .AddAlias<ITickFactory>();

            manager.AddScoped<ClientTickProvider>()
                .AddAlias<ITickProvider>()
                .AddAlias<ISubscriber>();

            manager.AddScoped<ServerTickProvider>()
                .AddAlias<ITickProvider>();

            services.AddScoped<ITickService, TickService>();

            manager.AddScoped<ClientStepProvider>()
                .AddAlias<IStepProvider>();

            manager.AddScoped<ServerStepProvider>()
                .AddAlias<IStepProvider>();

            services.AddScoped<IStepService, StepService>();

            manager.AddScoped<Lockstep.TickServerSystem>()
                .AddInterfaceAliases();

            manager.AddScoped<Lockstep.UserServerSystem>()
                .AddInterfaceAliases();

            services.AddNetMessageType<Tick>(DeliveryMethod.ReliableUnordered, 0);
        }

        private void ConfigurePredictive(IServiceCollection services, IServiceCollectionManager manager)
        {
            // manager.AddScoped<PredictiveSimulation>()
            //     .AddInterfaceAliases();
        }
    }
}
