using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Common.Descriptors
{
    [AutoLoad]
    public class DefaultTeamDescriptor : BaseTeamDescriptor
    {
        public DefaultTeamDescriptor()
        {
            this.WithTypeComponents([
                new ComponentBuilder<DefaultTeam>()
            ]);
        }
    }
}
