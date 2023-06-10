using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.DependencyInjection;
using Guppy.Common.DependencyInjection.Interfaces;
using Guppy.Loaders;
using Guppy.Network;
using LiteNetLib;
using Microsoft.Extensions.DependencyInjection;
using VoidHuntersRevived.Common.Constants;
using VoidHuntersRevived.Common.ECS.Systems;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Lockstep.Factories;
using VoidHuntersRevived.Common.Simulations.Lockstep.Providers;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations.Factories;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Messages;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Providers;
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

            services.AddScoped<ISimulationService, SimulationService>();

            services.ConfigureCollection(manager =>
            {
                this.ConfigureLockstep(services, manager);
                this.ConfigurePredictive(services, manager);
            });
        }

        private void ConfigureLockstep(IServiceCollection services, IServiceCollectionManager manager)
        {
            manager.AddScoped<LockstepSimulation>()
                .AddInterfaceAliases();

            manager.AddScoped<GameStateClient>()
                .AddInterfaceAliases();

            manager.AddScoped<GameStateServer>()
                .AddInterfaceAliases();

            manager.AddScoped<ClientTickFactory>()
                .AddAlias<ITickFactory>();

            manager.AddScoped<ServerTickFactory>()
                .AddAlias<ITickFactory>();

            manager.AddScoped<ClientTickProvider>()
                .AddAlias<ITickProvider>();

            manager.AddScoped<ServerTickProvider>()
                .AddAlias<ITickProvider>();

            manager.AddTransient<LockstepServer_UserSystem>()
                .AddAlias<ISystem>();

            services.AddNetMessageType<Tick>(DeliveryMethod.ReliableUnordered, 0);
            services.AddNetMessageType<EventDto>(DeliveryMethod.ReliableUnordered, 0);
            services.AddNetMessageType<StateBegin>(DeliveryMethod.ReliableUnordered, 0);
            services.AddNetMessageType<StateTick>(DeliveryMethod.ReliableUnordered, 0);
            services.AddNetMessageType<StateEnd>(DeliveryMethod.ReliableUnordered, 0);
        }

        private void ConfigurePredictive(IServiceCollection services, IServiceCollectionManager manager)
        {
        }
    }
}
