using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Common.Services
{
    public interface ITeamService
    {
        BelongsTo<Team, TeamMember> GetDefaultTeamComponent();
        BelongsTo<Team, TeamMember> GetOpenTeamComponent();
    }
}
