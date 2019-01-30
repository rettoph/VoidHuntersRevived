using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Extensions.Logging;
using VoidHuntersRevived.Client.Scenes;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts.Hulls;
using VoidHuntersRevived.Library.Interfaces;

namespace VoidHuntersRevived.Client.Entities.Drivers
{
    class ClientRemoteUserPlayerDriver : Entity, IUserPlayerDriver
    {
        private UserPlayer _parent;

        private ClientMainScene _scene;

        public ClientRemoteUserPlayerDriver(UserPlayer parent, EntityInfo info, IGame game) : base(info, game)
        {
            _parent = parent;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _scene = this.Scene as ClientMainScene;
        }

        public void Read(NetIncomingMessage im)
        {
            if (im.ReadBoolean())
            {
                var bridgeId = im.ReadInt64();

                if (_parent.Bridge == null || _parent.Bridge.Id != bridgeId)
                {
                    _parent.SetBridge(_scene.NetworkEntities.GetById(bridgeId) as Hull);
                }
            }
        }

        public void Write(NetOutgoingMessage om)
        {
            throw new NotImplementedException();
        }
    }
}
