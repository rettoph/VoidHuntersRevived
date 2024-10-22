using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Common.EntityTemplates
{
    public abstract class BaseTeamEntityTemplate : BaseEntityTemplate
    {
        public BaseTeamEntityTemplate(Key<IEntityTemplate> key) : base(key)
        {
            this.WithComponents([
                new Team(),
                new HasMany<TeamMember, Team>()
            ]);
        }
    }
}
