using Guppy.Core.Serialization.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Common.EntityTypes
{
    [PolymorphicJsonType<IEntityType>(nameof(VisibleTeamMemberEntityType))]
    public abstract class VisibleTeamMemberEntityType : TeamMemberEntityType
    {
        public VisibleTeamMemberEntityType(IKey<IEntityType> key, IKey<IEntityType>[] include) : base(key, include)
        {
            this.WithInstanceEntityComponents([
                new ColorScheme()
            ]);

            this.RequireTypeEntityComponents([
                typeof(ColorScheme)
            ]);
        }
    }
}
