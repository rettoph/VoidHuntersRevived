using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Engines;

namespace VoidHuntersRevived.Domain.Entities.Extensions
{
    internal static class IEngineServiceExtensions
    {
        public static void InitializeSimulationEngines(this IEngineService enginesService, ISimulation simulation)
        {
            Type simulationEngineType = typeof(ISimulationEngine<>).MakeGenericType(simulation.GetType());
            IEngine[] engines = enginesService.All().Where(x => x.GetType().IsAssignableTo(simulationEngineType)).ToArray();
            MethodInfo initializeMethod = simulationEngineType.GetMethod(nameof(ISimulationEngine<ISimulation>.Initialize), BindingFlags.Public | BindingFlags.Instance, new[] { simulation.GetType() }) ?? throw new NotImplementedException();
            object[] args = new[] { simulation };

            foreach (IEngine engine in engines)
            {
                initializeMethod.Invoke(engine, args);
            }
        }
    }
}
