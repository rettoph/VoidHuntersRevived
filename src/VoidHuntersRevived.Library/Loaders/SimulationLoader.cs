using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.DependencyInjection;
using Guppy.Common.DependencyInjection.Interfaces;
using Guppy.Loaders;
using LiteNetLib;
using Microsoft.Extensions.DependencyInjection;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Library.Constants;
using VoidHuntersRevived.Library.Simulations.Predictive;
using VoidHuntersRevived.Library.Simulations.Services;
using VoidHuntersRevived.Library.Simulations.Systems;

namespace VoidHuntersRevived.Library.Loaders
{
    [AutoLoad]
    internal sealed class SimulationLoader : IServiceLoader
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
            manager.AddScoped<Lockstep.LockstepSimulation>()
                .AddInterfaceAliases();

            manager.AddScoped<State>();

            manager.AddScoped<Lockstep.Services.ServerLockstepEventPublishingService>()
                .AddAlias<Lockstep.Services.ILockstepEventPublishingService>()
                .AddAlias<ISubscriber>();

            manager.AddScoped<Lockstep.Services.ClientLockstepEventPublishingService>()
                .AddAlias<Lockstep.Services.ILockstepEventPublishingService>();

            manager.AddScoped<Lockstep.Factories.ClientTickFactory>()
                .AddAlias<Lockstep.Factories.ITickFactory>();

            manager.AddScoped<Lockstep.Factories.ServerTickFactory>()
                .AddAlias<Lockstep.Factories.ITickFactory>();

            manager.AddScoped<Lockstep.Providers.ClientTickProvider>()
                .AddAlias<Lockstep.Providers.ITickProvider>()
                .AddAlias<ISubscriber>();

            manager.AddScoped<Lockstep.Providers.ServerTickProvider>()
                .AddAlias<Lockstep.Providers.ITickProvider>();

            services.AddScoped<Lockstep.Services.ITickService, Lockstep.Services.TickService>();

            manager.AddScoped<Lockstep.Providers.ClientStepProvider>()
                .AddAlias<Lockstep.Providers.IStepProvider>();

            manager.AddScoped<Lockstep.Providers.ServerStepProvider>()
                .AddAlias<Lockstep.Providers.IStepProvider>();

            services.AddScoped<Lockstep.Services.IStepService, Lockstep.Services.StepService>();

            manager.AddScoped<Lockstep.Systems.TickServerSystem>()
                .AddInterfaceAliases();

            manager.AddScoped<Lockstep.Systems.StateSystem>()
                .AddInterfaceAliases();

            manager.AddScoped<Lockstep.Systems.UserServerSystem>()
                .AddInterfaceAliases();

            services.AddNetMessageType<Tick>(DeliveryMethod.ReliableUnordered, 0);
            services.AddNetMessageType<Lockstep.Messages.ClientRequest>(DeliveryMethod.ReliableUnordered, 0);
            services.AddNetMessageType<Lockstep.Messages.StateBegin>(DeliveryMethod.ReliableUnordered, 0);
            services.AddNetMessageType<Lockstep.Messages.StateTick>(DeliveryMethod.ReliableUnordered, 0);
            services.AddNetMessageType<Lockstep.Messages.StateEnd>(DeliveryMethod.ReliableUnordered, 0);
        }

        private void ConfigurePredictive(IServiceCollection services, IServiceCollectionManager manager)
        {
            manager.AddScoped<PredictiveSimulation>()
                .AddInterfaceAliases();
        }
    }
}
