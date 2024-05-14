using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Teams.Common.Components;

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
