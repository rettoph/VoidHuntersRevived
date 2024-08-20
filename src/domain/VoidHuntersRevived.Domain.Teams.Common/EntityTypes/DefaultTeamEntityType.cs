using Guppy.Core.Serialization.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Teams.Common.EntityTypes
{
    [PolymorphicJsonType<IEntityType>(nameof(DefaultTeamEntityType))]
    public class DefaultTeamEntityType : BaseTeamEntityType
    {
        public DefaultTeamEntityType(IKey<DefaultTeamEntityType> key, IKey<IEntityType>[] include) : base(key, include)
        {
            this.WithTypeEntityComponents([
                new DefaultTeam(),
                new PrimitiveGroup()
            ]);
        }
    }
}
