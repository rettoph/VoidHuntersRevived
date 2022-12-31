using Guppy.Attributes;
using Guppy.Common.DependencyInjection;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Games;
using VoidHuntersRevived.Library.Maps;
using VoidHuntersRevived.Library.Providers;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Simulations.Systems.Lockstep;

namespace VoidHuntersRevived.Library.Loaders
{
    [AutoLoad]
    internal sealed class SimulationLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ISimulationService, SimulationService>()
                    .AddScoped<PredictiveSimulation>()
                    .AddScoped<LockstepSimulation>()
                    .AddScoped<SimulationState>()
                    .AddScoped<SimulatedEntityIdService>()
                    .AddScoped<UserIdSimulatedIdMap>();


            services.ConfigureCollection(manager =>
            {
                manager.AddScoped<StepService>()
                    .AddInterfaceAliases();

                manager.AddScoped<StepLocalProvider>()
                    .AddInterfaceAliases();

                manager.AddScoped<StepRemoteProvider>()
                    .AddInterfaceAliases();

                manager.AddScoped<LockstepTickRemoteMasterSystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<LockstepTickRemoteSlaveSystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<LockstepPilotSystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<LockstepUserPilotSystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<LockstepUserPilotRemoteMasterSystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<LockstepCurrentUserRemoteSlaveSystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<LockstepUserRemoteMasterSystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<LockstepPilotableAetherSystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<LockstepAetherSystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<LockstepAetherDebugRemoteMasterSystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<LockstepSimulationStateRemoteSlaveSystem>()
                    .AddInterfaceAliases();
            });
        }
    }
}
