using Guppy.Core.Serialization.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Common.EntityTemplates
{
    [PolymorphicJsonType<IEntityTemplate>(nameof(TeamEntityTemplate))]
    public class TeamEntityTemplate : BaseTeamEntityTemplate
    {
        public TeamEntityTemplate(Key<IEntityTemplate> key) : base(key)
        {
            this.WithComponents([
                new ColorScheme()
            ]);
        }
    }
}
