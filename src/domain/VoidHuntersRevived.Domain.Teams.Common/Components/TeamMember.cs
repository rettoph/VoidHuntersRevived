using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Utilities;

namespace VoidHuntersRevived.Domain.Teams.Common.Components
{
    public readonly struct TeamMember(EntityId teamEntityId) : IBelongsTo<Team, TeamMember>
    {
        public VhId BelongsToEntityVhId { get; } = teamEntityId.VhId;
        public FilterVhId<TeamMember> OwnerFilterId { get; } = new FilterVhId<TeamMember>(teamEntityId);
    }
}
