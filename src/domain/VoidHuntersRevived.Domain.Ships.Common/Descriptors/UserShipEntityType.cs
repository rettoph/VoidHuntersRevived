using Guppy.Core.Common.Attributes;
using Guppy.Core.Serialization.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Ships.Common.Descriptors
{
    [AutoLoad]
    [PolymorphicJsonType<IEntityType>(nameof(UserShipEntityType))]
    public class UserShipEntityType : ShipEntityType
    {
        public static readonly IKey<UserShipEntityType> UserShipEntityTypeKey = VoidHuntersRevived.Common.Key.GetByName<UserShipEntityType>(nameof(UserShipEntityType));

        public UserShipEntityType() : base(UserShipEntityType.UserShipEntityTypeKey, Array.Empty<IKey<IEntityType>>())
        {
            this.WithComponents([
                new UserId(),
            ]);
        }
    }
}
