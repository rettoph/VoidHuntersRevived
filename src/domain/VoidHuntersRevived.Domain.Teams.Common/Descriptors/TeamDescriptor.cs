using Guppy.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Common.Descriptors
{
    [AutoLoad]
    public class TeamDescriptor : VoidHuntersEntityDescriptor
    {
        public TeamDescriptor()
        {
            this.WithStaticComponents([
                new ComponentBuilder<Team>(),
                new ComponentBuilder<ColorScheme>()
            ]);
        }
    }
}
