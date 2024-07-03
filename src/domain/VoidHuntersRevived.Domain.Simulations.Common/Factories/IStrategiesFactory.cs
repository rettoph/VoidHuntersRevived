using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Common.Factories
{
    public interface IStrategiesFactory
    {
        IEnumerable<IStrategy> BuildStrategies(ISimulation simulation, StrategyTypeEnum[] strategies);
    }
}
