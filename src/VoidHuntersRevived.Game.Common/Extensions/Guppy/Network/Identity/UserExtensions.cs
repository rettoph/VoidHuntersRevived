using Guppy.Network.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Game.Common;
using VoidHuntersRevived.Game.Common.Components;

namespace Guppy.Network.Identity
{
    public static class UserExtensions
    {
        public static Guid GetUserShipId(this User user)
        {
            return EntityTypes.UserShip.Hash.Create(user.Id);
        }
    }
}
