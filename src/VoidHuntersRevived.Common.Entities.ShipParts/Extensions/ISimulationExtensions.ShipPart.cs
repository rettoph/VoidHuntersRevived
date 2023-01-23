using Microsoft.Extensions.DependencyInjection;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.ShipParts.Services;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Extensions
{
    public static partial class ISimulationExtensions
    {
        private static ConditionalWeakTable<ISimulation, IShipPartConfigurationService> _configurations = new();

        public static Entity CreateShipPart(this ISimulation simulation, ParallelKey key, string configuration)
        {
            var configurationInstance = simulation.ShipPartService().Get(configuration);
            return simulation.CreateEntity(key).MakeShipPart(configurationInstance);
        }

        private static IShipPartConfigurationService ShipPartService(this ISimulation simulation)
        {
            if(!_configurations.TryGetValue(simulation, out var service))
            {
                service = simulation.Provider.GetRequiredService<IShipPartConfigurationService>();
                _configurations.Add(simulation, service);
            }

            return service;
        }
    }
}
