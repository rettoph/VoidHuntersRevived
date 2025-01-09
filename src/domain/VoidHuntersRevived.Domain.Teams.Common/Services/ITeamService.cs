using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Common.Services
{
    public interface ITeamService
    {
        Team GetDefaultTeam();
        Team GetOpenTeam();
    }
}