using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Common.Services
{
    public interface ITeamService
    {
        bool TryGetGroupIndex(Id<Team> teamId, out GroupIndex groupIndex);

        Id<Team> GetDefaultTeamId();
        Id<Team> GetOpenTeamId();
    }
}
