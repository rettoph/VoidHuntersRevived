using Guppy.Core.Serialization.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Common.EntityTypes
{
    [PolymorphicJsonType<IEntityType>(nameof(VisibleTeamMemberEntityType))]
    public abstract class VisibleTeamMemberEntityType : TeamMemberEntityType
    {
        public VisibleTeamMemberEntityType(Key<IEntityType> key) : base(key)
        {
            this.RequireComponents([

                typeof(ColorScheme),
                typeof(PrimitiveSequence)
            ]);
        }
    }
}
