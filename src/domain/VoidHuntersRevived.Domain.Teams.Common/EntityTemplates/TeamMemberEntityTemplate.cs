using Guppy.Core.Serialization.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Common.EntityTemplates
{
    [PolymorphicJsonType<IEntityTemplate>(nameof(TeamMemberEntityTemplate))]
    public abstract class TeamMemberEntityTemplate : Entities.Common.BaseEntityTemplate
    {
        public TeamMemberEntityTemplate(Key<IEntityTemplate> key) : base(key)
        {
            this.WithComponents([
                new TeamMember(),
                new BelongsTo<Team, TeamMember>()
            ]);
        }
    }
}
