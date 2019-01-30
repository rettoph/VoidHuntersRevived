using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using Lidgren.Network.Xna;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Interfaces;

namespace VoidHuntersRevived.Client.Entities.Drivers
{
    class ClientRemoteUserPlayerDriver : Entity, IUserPlayerDriver
    {
        private UserPlayer _parent;

        public ClientRemoteUserPlayerDriver(UserPlayer parent, EntityInfo info, IGame game) : base(info, game)
        {
            _parent = parent;
        }

        public void Read(NetIncomingMessage im)
        {
            _parent.TractorBeam.Read(im);
        }

        public void Write(NetOutgoingMessage om)
        {
            _parent.TractorBeam.Write(om);
        }
    }
}
