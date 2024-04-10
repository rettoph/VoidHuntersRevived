using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Common.Services
{
    public interface ITeamService
    {
        Id<Team> GetDefaultTeamId();
        Id<Team> GetOpenTeamId();
    }
}
