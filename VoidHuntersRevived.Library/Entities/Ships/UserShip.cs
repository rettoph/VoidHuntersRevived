using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Library.Entities.Ships
{
    public class UserShip : Ship
    {
        public IUser User { get; private set; }

        public UserShip(IUser user, EntityInfo info, IGame game) : base(info, game)
        {
            this.User = user;
        }
    }
}
