using VoidHuntersRevived.Tests.Common.Entities.Interfaces;
using VoidHuntersRevived.Tests.Common.Teams.Mockers;

namespace VoidHuntersRevived.Tests.Common.Teams.Interfaces
{
    public interface ITeamSimulationMocker : IEntitySimulationMocker
    {
        TeamLockstepStrategyMocker TeamLockstepStrategyMocker { get; }
        TeamPredictiveStrategyMocker TeamPredictiveStrategyMocker { get; }
    }
    public interface ITeamSimulationMocker<out TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker> : IEntitySimulationMocker<TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker>, ITeamSimulationMocker
        where TSelf : ITeamSimulationMocker<TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker>
        where TStrategyMocker : ITeamStrategyMocker
        where TLockstepStrategyMocker : TeamLockstepStrategyMocker, TStrategyMocker, new()
        where TPredictiveStrategyMocker : TeamPredictiveStrategyMocker, TStrategyMocker, new()
    {
    }
}
