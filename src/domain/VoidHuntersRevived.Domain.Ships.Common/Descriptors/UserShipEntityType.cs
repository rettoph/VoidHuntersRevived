using Guppy.Core.Common.Attributes;
using Guppy.Core.Serialization.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Constants;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Ships.Common.Descriptors
{
    [AutoLoad]
    [PolymorphicJsonType<IEntityType>(nameof(UserShipEntityType))]
    public class UserShipEntityType : ShipEntityType
    {
        public static readonly Key<IEntityType> UserShipEntityTypeKey = Key<IEntityType>.GetByName(nameof(UserShipEntityType));

        public UserShipEntityType() : base(UserShipEntityType.UserShipEntityTypeKey)
        {
            this.WithComponents([
                new UserId(),
                new Awake(sleepingAllowed: false),
                new Collision()
                {
                    Categories = CollisionGroups.ShipCategories,
                    CollidesWith = CollisionGroups.ShipCollidesWith
                },
                new PhysicsBubble()
                {
                    Enabled = true,
                    Radius = (Fix64)25
                }
            ]);
        }
    }
}
