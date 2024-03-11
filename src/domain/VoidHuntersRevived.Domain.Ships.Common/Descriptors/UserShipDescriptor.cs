using Svelto.ECS;
using VoidHuntersRevived.Domain.Ships.Common.Components;

namespace VoidHuntersRevived.Domain.Ships.Common.Descriptors
{
    public class UserShipDescriptor : ShipDescriptor
    {
        public UserShipDescriptor()
        {
            this.WithInstanceComponents([
                new ComponentBuilder<UserId>(),
            ]);
        }
    }
}
