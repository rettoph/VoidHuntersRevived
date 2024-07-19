using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Descriptors;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Common.Descriptors
{
    public abstract class BaseTeamDescriptor : VoidHuntersEntityDescriptor
    {
        public BaseTeamDescriptor()
        {
            this.WithTypeComponents([
                new ComponentBuilder<Team>(),
                new ComponentBuilder<HasMany<TeamMember, Team>>()
            ]);
        }
    }
}
