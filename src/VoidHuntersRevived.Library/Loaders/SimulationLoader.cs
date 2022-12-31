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
using VoidHuntersRevived.Library.Simulations.Systems.Predictive;
using VoidHuntersRevived.Library.Simulations.Systems.Shared;

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

                // --- Shared Systems ---
                manager.AddScoped<PilotSystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<UserPilotSystem>()
                    .AddInterfaceAliases();


                // -- Lockstep Systems ---
                manager.AddScoped<LockstepTickRemoteMasterSystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<LockstepTickRemoteSlaveSystem>()
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

                // --- Predictive Systems ---
                manager.AddScoped<PredictivePilotableAetherSystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<PredictiveCurrentUserSystem>()
                    .AddInterfaceAliases();
            });
        }
    }
}
