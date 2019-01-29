﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Interfaces;
using VoidHuntersRevived.Library.Interfaces;

namespace VoidHuntersRevived.Server.Drivers
{
    public class ServerFarseerEntityDriver : IFarseerEntityDriver
    {
        private NetworkedFarseerEntity _parent;

        public Vector2 Position { get { return _parent.Body.Position; } set { _parent.Body.Position = value; } }
        public Vector2 LinearVelocity { get { return _parent.Body.LinearVelocity; } set { _parent.Body.LinearVelocity = value; } }
        public Single Rotation { get { return _parent.Body.Rotation; } set { _parent.Body.Rotation = value; } }
        public Single AngularVelocity { get { return _parent.Body.AngularVelocity; } set { _parent.Body.AngularVelocity = value; } }

        public ServerFarseerEntityDriver(NetworkedFarseerEntity parent)
        {
            _parent = parent;
        }

        public void Update(GameTime gameTime)
        {
            // No need for lerping on the server :)
            // throw new NotImplementedException();
            if (_parent.Body.Awake)
                _parent.Dirty = true;
        }
    }
}
