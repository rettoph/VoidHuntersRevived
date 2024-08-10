using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Common.EntityTypes
{
    public abstract class BaseTeamEntityType : EntityType
    {
        public BaseTeamEntityType(IKey<IEntityType> key, IKey<IEntityType>[] include) : base(key, include)
        {
            this.WithTypeEntityComponents([
                new Team(),
                new HasMany<TeamMember, Team>()
            ]);
        }
    }
}
