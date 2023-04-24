using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.DependencyInjection;
using Guppy.Common.DependencyInjection.Interfaces;
using Guppy.Loaders;
using LiteNetLib;
using Microsoft.Extensions.DependencyInjection;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations.Factories;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Messages;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Providers;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Services;
using VoidHuntersRevived.Domain.Simulations.Predictive;
using VoidHuntersRevived.Domain.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations.Systems;

namespace VoidHuntersRevived.Domain.Simulations.Loaders
{
    [AutoLoad]
    public sealed class SimulationLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSetting<TimeSpan>(Settings.StepInterval, TimeSpan.FromMilliseconds(20), false);
            services.AddSetting<int>(Settings.StepsPerTick, 3, false);

            services.AddSingleton<IGlobalSimulationService, GlobalSimulationService>()
                    .AddScoped<ISimulationService, SimulationService>()
                    .AddScoped<IParallelableService, ParallelableService>();

            services.ConfigureCollection(manager =>
            {
                manager.AddScoped<Server_InputSystem>()
                    .AddInterfaceAliases();

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
            manager.AddScoped<LockstepClientSimulation>()
                .AddInterfaceAliases();

            manager.AddScoped<LockstepServerSimulation>()
                .AddInterfaceAliases();

            manager.AddScoped<State>();

            manager.AddScoped<ClientTickFactory>()
                .AddAlias<ITickFactory>();

            manager.AddScoped<DefaultTickFactory>()
                .AddAlias<ITickFactory>();

            manager.AddScoped<ClientTickProvider>()
                .AddAlias<ITickProvider>()
                .AddAlias<ISubscriber>();

            manager.AddScoped<DefaultTickProvider>()
                .AddAlias<ITickProvider>();

            services.AddScoped<ITickService, TickService>();

            manager.AddScoped<ClientStepProvider>()
                .AddAlias<IStepProvider>();

            manager.AddScoped<DefaultStepProvider>()
                .AddAlias<IStepProvider>();

            services.AddScoped<IStepService, StepService>();

            manager.AddScoped<LockstepServer_TickSystem>()
                .AddInterfaceAliases();

            manager.AddScoped<Lockstep_StateSystem>()
                .AddInterfaceAliases();

            manager.AddScoped<LockstepServer_UserSystem>()
                .AddInterfaceAliases();

            services.AddNetMessageType<Tick>(DeliveryMethod.ReliableUnordered, 0);
            services.AddNetMessageType<InputDto>(DeliveryMethod.ReliableUnordered, 0);
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
