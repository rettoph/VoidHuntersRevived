using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Ships.Components;
using VoidHuntersRevived.Common.Ships.Serialization.Components;

namespace VoidHuntersRevived.Common.Ships.Descriptors
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
