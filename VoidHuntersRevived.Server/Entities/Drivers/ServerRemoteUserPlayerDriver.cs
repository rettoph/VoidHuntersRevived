using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Extensions.Logging;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Interfaces;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Interfaces;
using VoidHuntersRevived.Networking.Scenes;

namespace VoidHuntersRevived.Server.Entities.Drivers
{
    class ServerRemoteUserPlayerDriver : Entity, IUserPlayerDriver
    {
        private UserPlayer _parent;

        public ServerRemoteUserPlayerDriver(UserPlayer parent, EntityInfo info, IGame game) : base(info, game)
        {
            _parent = parent;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        public void Read(NetIncomingMessage im)
        {
            // We must update all clients of this development
            _parent.Dirty = true;
        }

        public void Write(NetOutgoingMessage om)
        {
            if(_parent.Bridge == null)
            {
                om.Write(false);
            }
            else
            {
                om.Write(true);
                om.Write(_parent.Bridge.Id);
            }
        }
    }
}
