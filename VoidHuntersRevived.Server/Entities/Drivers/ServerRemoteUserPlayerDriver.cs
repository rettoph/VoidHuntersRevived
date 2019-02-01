using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Interfaces;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Interfaces;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Networking.Scenes;

namespace VoidHuntersRevived.Server.Entities.Drivers
{
    class ServerRemoteUserPlayerDriver : Entity, IUserPlayerDriver
    {
        private UserPlayer _parent;
        private MainScene _scene;

        public ServerRemoteUserPlayerDriver(UserPlayer parent, EntityInfo info, IGame game) : base(info, game)
        {
            _parent = parent;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _scene = this.Scene as MainScene;
        }

        public void Read(NetIncomingMessage im)
        {
            if (im.SenderConnection.RemoteUniqueIdentifier == _parent.User.Id)
            {
                _parent.Movement[0] = im.ReadBoolean();
                _parent.Movement[1] = im.ReadBoolean();
                _parent.Movement[2] = im.ReadBoolean();
                _parent.Movement[3] = im.ReadBoolean();

                _parent.TractorBeam.Body.Position = im.ReadVector2();

                /* BEGIN READ TRACTOR BEAM SETTINGS */
                if (im.ReadBoolean())
                { // If the client requests a tractor beam connection...
                    if (_parent.TractorBeam.Connection == null)
                    { // If the tractor beam isnt already connected to something...
                        var targetId = im.ReadInt64();
                        var target = _scene.NetworkEntities.GetById(targetId) as ITractorableEntity;

                        _parent.TractorBeam.CreateConnection(target);
                    }
                }
                else if(_parent.TractorBeam.Connection != null)
                { // If the client requests a tractor beam disconnect...
                    var node = _parent.AvailableFemaleConnectionNodes
                        .OrderBy(fn => Vector2.Distance(fn.WorldPoint, _parent.TractorBeam.Body.Position)).First();

                    if (Vector2.Distance(node.WorldPoint, _parent.TractorBeam.Body.Position) < 0.5)
                    {
                        var target = _parent.TractorBeam.Connection.Target as ShipPart;

                        target.AttatchTo(node);
                    }

                    _parent.TractorBeam.Connection.Disconnect();
                }

                // We must update all clients of this development
                _parent.Dirty = true;
            }
            else
            { // if the sender connection doesnt match the players user id...
                this.Game.Logger.LogCritical($"SenderConnection/UserId mismatch, kicking connection");
                im.SenderConnection.Disconnect("SenderConnection/UserId mismatch");
            }
        }

        public void Write(NetOutgoingMessage om)
        {
            // Write the bridge settings, if a bridge exists
            if(_parent.Bridge == null)
            {
                om.Write(false);
            }
            else
            {
                om.Write(true);
                om.Write(_parent.Bridge.Id);
            }

            // Write the tractor beam settings, if a tractor beam exists
            if (_parent.TractorBeam == null)
            {
                om.Write(false);
            }
            else
            {
                om.Write(true);
                _parent.TractorBeam.Write(om);
            }
        }
    }
}
