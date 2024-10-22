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
    [PolymorphicJsonType<IEntityTemplate>(nameof(UserShipEntityTemplate))]
    public class UserShipEntityTemplate : ShipEntityTemplate
    {
        public static readonly Key<IEntityTemplate> UserShipEntityTemplateKey = Key<IEntityTemplate>.GetByName(nameof(UserShipEntityTemplate));

        public UserShipEntityTemplate() : base(UserShipEntityTemplate.UserShipEntityTemplateKey)
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
