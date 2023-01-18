using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.DependencyInjection;
using Guppy.Common.DependencyInjection.Interfaces;
using Guppy.Loaders;
using LiteNetLib;
using Microsoft.Extensions.DependencyInjection;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Factories;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Messages;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Providers;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Services;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Systems;
using VoidHuntersRevived.Domain.Simulations.Predictive;
using VoidHuntersRevived.Domain.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations.Systems;

namespace VoidHuntersRevived.Domain.Simulations.Loaders
{
    public sealed class SimulationLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSetting<TimeSpan>(Settings.StepInterval, TimeSpan.FromMilliseconds(20), false);
            services.AddSetting<int>(Settings.StepsPerTick, 3, false);

            services.AddSingleton<IGlobalSimulationService, GlobalSimulationService>()
                    .AddScoped<ISimulationService, SimulationService>()
                    .AddScoped<IParallelService, ParallelService>();

            services.ConfigureCollection(manager =>
            {
                manager.AddScoped<AetherSystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<PilotingSystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<PilotableSystem>()
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

            manager.AddScoped<State>();

            manager.AddScoped<ServerLockstepInputService>()
                .AddInterfaceAliases();

            manager.AddScoped<ClientLockstepInputService>()
                .AddInterfaceAliases();

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

            manager.AddScoped<TickServerSystem>()
                .AddInterfaceAliases();

            manager.AddScoped<StateSystem>()
                .AddInterfaceAliases();

            manager.AddScoped<UserServerSystem>()
                .AddInterfaceAliases();

            services.AddNetMessageType<Tick>(DeliveryMethod.ReliableUnordered, 0);
            services.AddNetMessageType<ClientRequest>(DeliveryMethod.ReliableUnordered, 0);
            services.AddNetMessageType<StateBegin>(DeliveryMethod.ReliableUnordered, 0);
            services.AddNetMessageType<StateTick>(DeliveryMethod.ReliableUnordered, 0);
            services.AddNetMessageType<StateEnd>(DeliveryMethod.ReliableUnordered, 0);
        }

        private void ConfigurePredictive(IServiceCollection services, IServiceCollectionManager manager)
        {
            manager.AddScoped<PredictiveSimulation>()
                .AddInterfaceAliases();
        }
    }
}
