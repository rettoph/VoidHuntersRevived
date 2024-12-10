using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Common.Components
{
    public readonly struct TeamMember(EntityId teamEntityId) : IBelongsTo<Team, TeamMember>
    {
        public EntityFilterId<TeamMember> ParentFilterId { get; } = new EntityFilterId<TeamMember>(teamEntityId.EGID);
    }
}
