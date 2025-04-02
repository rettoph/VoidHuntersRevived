using VoidHuntersRevived.Tests.Common.Entities.Interfaces;
using VoidHuntersRevived.Tests.Common.Teams.Builders;

namespace VoidHuntersRevived.Tests.Common.Teams.Interfaces
{
    public interface ITeamStrategyMocker : IEntityStrategyMocker
    {
        TeamServiceBuilder TeamServiceBuilder { get; }
    }
}
