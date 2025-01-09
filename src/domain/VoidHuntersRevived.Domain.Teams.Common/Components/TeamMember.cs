using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Common.Components
{
    public readonly struct TeamMember(EntityLocalId teamLocalId) : IBelongsTo<Team, TeamMember>
    {
        public EntityFilterId<TeamMember> ParentFilterId { get; } = EntityFilterId<TeamMember>.Create<Team>(teamLocalId.Value);
    }
}