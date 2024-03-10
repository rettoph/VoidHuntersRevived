using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Serialization.Components;

namespace VoidHuntersRevived.Domain.Ships.Common.Descriptors
{
    public class UserShipDescriptor : ShipDescriptor
    {
        public UserShipDescriptor()
        {
            this.WithInstanceComponents(new[]
            {
                new ComponentManager<UserId, UserIdComponentSerializer>(),
            });
        }
    }
}
