using Guppy.Core.Serialization.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.EntityTypes;

namespace VoidHuntersRevived.Domain.Physics.Common.EntityTypes
{
    [PolymorphicJsonType<IEntityType>(nameof(BodyEntityType))]
    public abstract class BodyEntityType : TeamMemberEntityType
    {
        public BodyEntityType(IKey<IEntityType> key, IKey<IEntityType>[] include) : base(key, include)
        {
            this.WithComponents([
                new Location(),
                new Enabled(),
                new Awake(true)
            ]);

            this.RequireComponents([
                typeof(Collision)
            ]);
        }
    }
}
