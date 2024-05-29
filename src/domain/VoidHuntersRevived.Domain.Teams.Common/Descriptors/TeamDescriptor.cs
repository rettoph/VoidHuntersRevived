using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Descriptors;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Common.Descriptors
{
    [AutoLoad]
    public class TeamDescriptor : VoidHuntersEntityDescriptor
    {
        public TeamDescriptor()
        {
            this.WithTypeComponents([
                new ComponentBuilder<Team>(),
                new ComponentBuilder<ColorScheme>(),
                new ComponentBuilder<HasMany<TeamMember, Team>>()
            ]);
        }
    }
}
