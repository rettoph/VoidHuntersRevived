using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Descriptors;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Common.Descriptors
{
    public abstract class TeamMemberEntityDescriptor : VoidHuntersEntityDescriptor
    {
        public TeamMemberEntityDescriptor()
        {
            this.WithInstanceComponents([
                new ComponentBuilder<TeamMember>(),
                new ComponentBuilder<ColorScheme>()
            ]);

            this.WithTypeComponents([
                new ComponentBuilder<ColorScheme>()
            ]);
        }
    }
}
