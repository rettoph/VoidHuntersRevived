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
using VoidHuntersRevived.Library.Entities.ShipParts;
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

        private Boolean _tractorBeamState;
        private Int64 _tractorBeamTargetId;

        public ClientLocalUserPlayerDriver(UserPlayer parent, EntityInfo info, IGame game) : base(info, game)
        {
            _parent = parent;

            this.Enabled = false;
            this.Visible = false;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _scene = this.Scene as ClientMainScene;

            _cursor = _scene.Cursor;
            _camera = _scene.Camera;
            _scene.CurrentPlayer = _parent;
            _parent.UpdateOrder = 10000; // Increase update order, to ensure client gets updated last

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

            //_parent.TractorBeam.Body.Position = _cursor.Body.Position;

            switch (Mouse.GetState().RightButton)
            {
                case ButtonState.Released:
                    if (_parent.TractorBeam.Connection != null)
                    {
                        _tractorBeamState = false;
                        _parent.Dirty = true;
                    }
                    break;
                case ButtonState.Pressed:
                    if(_parent.TractorBeam.Connection == null && _cursor.Over is ITractorableEntity)
                    {
                        _tractorBeamState = true;
                        _tractorBeamTargetId = (_cursor.Over as ITractorableEntity).Id;
                        _parent.Dirty = true;
                    }  
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

            // TractorBeam Target Preview Rendering
            // The following code will place the tractorbeams current target (if any)
            // Onto the nearest available female node
            if(_parent.TractorBeam.Connection != null && _parent.TractorBeam.Connection.Target is ShipPart && _parent.AvailableFemaleConnectionNodes?.Length > 0)
            { // Only proceed if there is a connection and there are open female nodes
                var node = _parent.AvailableFemaleConnectionNodes
                    .OrderBy(fn => Vector2.Distance(fn.WorldPoint, _parent.TractorBeam.Body.Position)).First();

                if(Vector2.Distance(node.WorldPoint, _parent.TractorBeam.Body.Position) < 1)
                {
                    var target = _parent.TractorBeam.Connection.Target as ShipPart;

                    _parent.TractorBeam.Connection.Target.Body.Rotation = _parent.Bridge.Body.Rotation + node.LocalRotation + target.MaleConnectionNode.LocalRotation;
                    target.UpdateRotationMatrix(); // Update the targets rotation matrix

                    _parent.TractorBeam.Connection.Target.Body.Position = _parent.Bridge.Body.Position + Vector2.Transform(node.LocalPoint, node.Owner.RotationMatrix) - Vector2.Transform(target.MaleConnectionNode.LocalPoint, target.RotationMatrix);
                    

                    
                }

            }


            // Update the camera position to follow the clients bridge
            if(_parent.Bridge != null)
            {
                _camera.Position = _parent.Bridge.Body.Position;
            }

            _parent.Dirty = true;
        }

        public void Read(NetIncomingMessage im)
        {
            if(im.ReadBoolean())
            { // Read the bridge settings
                var bridgeId = im.ReadInt64();

                if (_parent.Bridge == null || _parent.Bridge.Id != bridgeId)
                {
                    _parent.SetBridge(_scene.NetworkEntities.GetById(bridgeId) as Hull);   
                }
            }

            if(im.ReadBoolean())
            { // Read the tractor beam settings
                _parent.TractorBeam.Read(im);
            }
        }

        public void Write(NetOutgoingMessage om)
        {
            om.Write(_movement[0]);
            om.Write(_movement[1]);
            om.Write(_movement[2]);
            om.Write(_movement[3]);

            // Write the cursor position
            om.Write(_cursor.Body.Position);

            // Write tractor beam settings
            om.Write(_tractorBeamState);
            om.Write(_tractorBeamTargetId);
        }
    }
}
