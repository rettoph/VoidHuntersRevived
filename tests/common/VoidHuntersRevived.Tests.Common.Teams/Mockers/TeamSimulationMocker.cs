using VoidHuntersRevived.Tests.Common.Entities.Mockers;
using VoidHuntersRevived.Tests.Common.Teams.Interfaces;

namespace VoidHuntersRevived.Tests.Common.Teams.Mockers
{
    public abstract class TeamSimulationMocker<TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker> : EntitySimulationMocker<TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker>, ITeamSimulationMocker<TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker>
        where TSelf : TeamSimulationMocker<TSelf, TStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker>
        where TStrategyMocker : ITeamStrategyMocker
        where TLockstepStrategyMocker : TeamLockstepStrategyMocker, TStrategyMocker, new()
        where TPredictiveStrategyMocker : TeamPredictiveStrategyMocker, TStrategyMocker, new()
    {
        TeamLockstepStrategyMocker ITeamSimulationMocker.TeamLockstepStrategyMocker => this.LockstepStrategyMocker;
        TeamPredictiveStrategyMocker ITeamSimulationMocker.TeamPredictiveStrategyMocker => this.PredictiveStrategyMocker;
    }

    public sealed class TeamSimulationMocker<TLockstepStrategyMocker, TPredictiveStrategyMocker> : TeamSimulationMocker<TeamSimulationMocker<TLockstepStrategyMocker, TPredictiveStrategyMocker>, ITeamStrategyMocker, TLockstepStrategyMocker, TPredictiveStrategyMocker>
        where TLockstepStrategyMocker : TeamLockstepStrategyMocker, ITeamStrategyMocker, new()
        where TPredictiveStrategyMocker : TeamPredictiveStrategyMocker, ITeamStrategyMocker, new()
    {
    }
}
