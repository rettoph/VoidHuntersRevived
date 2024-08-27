using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Common.EntityTypes
{
    public abstract class BaseTeamEntityType : BaseEntityType
    {
        public BaseTeamEntityType(Key<IEntityType> key) : base(key)
        {
            this.WithComponents([
                new Team(),
                new HasMany<TeamMember, Team>()
            ]);
        }
    }
}
