using Guppy.Core.Serialization.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Common.EntityTypes
{
    [PolymorphicJsonType<IEntityType>(nameof(TeamEntityType))]
    public class TeamEntityType : BaseTeamEntityType
    {
        public TeamEntityType(IKey<TeamEntityType> key, IKey<IEntityType>[] include) : base(key, include)
        {
            this.WithTypeEntityComponents([
                new ColorScheme(),
                new PrimitiveGroup()
            ]);
        }
    }
}
