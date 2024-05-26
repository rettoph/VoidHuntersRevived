using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Common.Descriptors
{
    public abstract class TeamMemberEntityDescriptor : VoidHuntersEntityDescriptor
    {
        public TeamMemberEntityDescriptor()
        {
            this.WithInstanceComponents([
                new ComponentBuilder<Id<Team>>(),
                new ComponentBuilder<GroupIndex<Team>>(),
                new ComponentBuilder<ColorScheme>()
            ]);

            this.WithTypeComponents([
                new ComponentBuilder<ColorScheme>()
            ]);
        }
    }
}
