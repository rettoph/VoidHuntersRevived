using Guppy.Network.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Game;
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
