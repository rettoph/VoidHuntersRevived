using Guppy.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Ships.Common.Components;

namespace VoidHuntersRevived.Domain.Ships.Common.Descriptors
{
    [AutoLoad]
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
