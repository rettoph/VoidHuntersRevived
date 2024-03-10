using Svelto.ECS;
using System.Reflection;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

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
