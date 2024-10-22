using Guppy.Core.Serialization.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Common.EntityTemplates
{
    [PolymorphicJsonType<IEntityTemplate>(nameof(VisibleTeamMemberEntityTemplate))]
    public abstract class VisibleTeamMemberEntityTemplate : TeamMemberEntityTemplate
    {
        public VisibleTeamMemberEntityTemplate(Key<IEntityTemplate> key) : base(key)
        {
            this.RequireComponents([

                typeof(ColorScheme)
            ]);
        }
    }
}
