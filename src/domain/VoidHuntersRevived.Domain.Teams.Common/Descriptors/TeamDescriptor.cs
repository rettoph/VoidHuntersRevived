using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Common.Descriptors
{
    [AutoLoad]
    public class TeamDescriptor : BaseTeamDescriptor
    {
        public TeamDescriptor()
        {
            this.WithTypeComponents([
                new ComponentBuilder<ColorScheme>(),
                new ComponentBuilder<zIndex>()
            ]);
        }
    }
}
