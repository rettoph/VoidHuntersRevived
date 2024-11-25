using Autofac;
using Guppy.Core.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;

namespace VoidHuntersRevived.Domain.Simulations.Modules
{
    public class StrategyEngineFilterModule(IAssemblyService assemblyService) : Module
    {
        private readonly IAssemblyService _assemblyService = assemblyService;

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            foreach (Type strategyType in _assemblyService.GetTypes<IStrategy>())
            {
                Type strategyEngineType = typeof(StrategyEngine<>).MakeGenericType(strategyType);

                builder.RegisterStrategyFilter(strategyEngineType, strategyType);
            }
        }
    }
}
