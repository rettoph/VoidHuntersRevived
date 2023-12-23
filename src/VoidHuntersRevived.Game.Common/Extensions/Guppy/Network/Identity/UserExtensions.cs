using VoidHuntersRevived.Common;
using VoidHuntersRevived.Game.Common;

namespace Guppy.Network.Identity
{
    public static class UserExtensions
    {
        public static VhId GetUserShipId(this User user)
        {
            return EntityTypes.UserShip.Id.Value.Create(user.Id);
        }
    }
}
