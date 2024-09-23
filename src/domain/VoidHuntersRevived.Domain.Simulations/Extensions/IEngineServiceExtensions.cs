using Guppy.Core.Common.Extensions;
using Svelto.ECS;
using System.Reflection;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Extensions
{
    internal static class IEngineServiceExtensions
    {
        public static void InitializeStrategyEngines(this IEngineService enginesService, IStrategy strategy)
        {
            Type simulationEngineType = typeof(IStrategyEngine<>).MakeGenericType(strategy.GetType());
            IEngine[] engines = enginesService.All().Where(x => x.GetType().IsAssignableTo(simulationEngineType)).Sequence<IEngine, EngineSequence>(true).ToArray();
            MethodInfo initializeMethod = simulationEngineType.GetMethod(nameof(IStrategyEngine<IStrategy>.Initialize), BindingFlags.Public | BindingFlags.Instance, new[] { strategy.GetType() }) ?? throw new NotImplementedException();
            object[] args = new[] { strategy };

            foreach (IEngine engine in engines)
            {
                initializeMethod.Invoke(engine, args);
                // scheduler.SubmitEntities();
            }
        }
    }
}
