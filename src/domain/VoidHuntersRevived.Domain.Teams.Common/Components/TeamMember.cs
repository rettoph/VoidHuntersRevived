using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Utilities;

namespace VoidHuntersRevived.Domain.Teams.Common.Components
{
    public readonly struct TeamMember(EntityId teamEntityId) : IBelongsTo<Team, TeamMember>
    {
        public FilterVhId<TeamMember> ParentFilterId { get; } = new FilterVhId<TeamMember>(teamEntityId);
    }
}
