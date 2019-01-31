using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using VoidHuntersRevived.Client.Scenes;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Interfaces;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts.Hulls;
using VoidHuntersRevived.Library.Interfaces;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Client.Entities.Drivers
{
    public class ClientLocalUserPlayerDriver : Entity, IUserPlayerDriver
    {
        private UserPlayer _parent;
        private Cursor _cursor;

        private ClientMainScene _scene;
        private Camera _camera;
        private Boolean[] _movement;

        public ClientLocalUserPlayerDriver(UserPlayer parent, EntityInfo info, IGame game) : base(info, game)
        {
            _parent = parent;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _scene = this.Scene as ClientMainScene;

            _cursor = _scene.Cursor;
            _camera = _scene.Camera;
            _movement = new Boolean[]
            {
                _parent.Movement[0],
                _parent.Movement[1],
                _parent.Movement[2],
                _parent.Movement[3]
            };
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _parent.TractorBeam.Body.Position = _cursor.Body.Position;

            switch (Mouse.GetState().RightButton)
            {
                case ButtonState.Released:
                    if (_parent.TractorBeam.SelectedEntity != null)
                    {
                        _parent.TractorBeam.TryRelease();
                        _parent.Dirty = true;
                    }
                    break;
                case ButtonState.Pressed:
                    if(_parent.TractorBeam.SelectedEntity == null && _cursor.Over is ITractorableEntity)
                    {
                        _parent.TractorBeam.TrySelect(_cursor.Over as ITractorableEntity);
                    }

                    _parent.Dirty = true;
                    break;
            }

            var keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.W) != _movement[0])
            {
                _movement[0] = !_movement[0];
                _parent.Dirty = true;
            }
            if (keyboard.IsKeyDown(Keys.D) != _movement[1])
            {
                _movement[1] = !_movement[1];
                _parent.Dirty = true;
            }
            if (keyboard.IsKeyDown(Keys.S) != _movement[2])
            {
                _movement[2] = !_movement[2];
                _parent.Dirty = true;
            }
            if (keyboard.IsKeyDown(Keys.A) != _movement[3])
            {
                _movement[3] = !_movement[3];
                _parent.Dirty = true;
            }

            // Update the camera position to follow the clients bridge
            if(_parent.Bridge != null)
            {
                _camera.Position = _parent.Bridge.Body.Position;
            }
        }

        public void Read(NetIncomingMessage im)
        {
            if(im.ReadBoolean())
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
            om.Write(_movement[0]);
            om.Write(_movement[1]);
            om.Write(_movement[2]);
            om.Write(_movement[3]);
        }
    }
}
