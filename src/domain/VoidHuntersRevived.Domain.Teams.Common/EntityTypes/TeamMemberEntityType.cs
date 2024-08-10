using Guppy.Core.Serialization.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Common.EntityTypes
{
    [PolymorphicJsonType<IEntityType>(nameof(TeamMemberEntityType))]
    public abstract class TeamMemberEntityType : EntityType
    {
        public TeamMemberEntityType(IKey<IEntityType> key, IKey<IEntityType>[] include) : base(key, include)
        {
            this.WithInstanceEntityComponents([
                new TeamMember(),
                new BelongsTo<Team, TeamMember>()
            ]);
        }
    }
}
