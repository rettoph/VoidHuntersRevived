using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Interfaces;

namespace VoidHuntersRevived.Server.Entities.Drivers
{
    class ServerRemoteUserPlayerDriver : Entity, IUserPlayerDriver
    {
        private UserPlayer _parent;

        public ServerRemoteUserPlayerDriver(UserPlayer parent, EntityInfo info, IGame game) : base(info, game)
        {
            _parent = parent;
        }
    }
}
