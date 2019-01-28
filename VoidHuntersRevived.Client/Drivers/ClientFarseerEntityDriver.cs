using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Entities.Interfaces;
using VoidHuntersRevived.Library.Interfaces;

namespace VoidHuntersRevived.Client.Drivers
{
    class ClientFarseerEntityDriver : IFarseerEntityDriver
    {
        public Vector2 Position { get; set; }
        public Vector2 LinearVelocity { get; set; }
        public Single Rotation { get; set; }
        public Single AngularVelocity { get; set; }

        private IFarseerEntity _parent;
        private Single _lerpStrength = 0.005f;

        public ClientFarseerEntityDriver(IFarseerEntity parent)
        {
            _parent = parent;
        }

        public void Update(GameTime gameTime)
        {
            _parent.Body.Position = Vector2.Lerp(_parent.Body.Position, this.Position, _lerpStrength);
            _parent.Body.LinearVelocity = Vector2.Lerp(_parent.Body.LinearVelocity, this.LinearVelocity, _lerpStrength);

            _parent.Body.Rotation = MathHelper.Lerp(_parent.Body.Rotation, this.Rotation, _lerpStrength);
            _parent.Body.AngularVelocity = MathHelper.Lerp(_parent.Body.AngularVelocity, this.AngularVelocity, _lerpStrength);
        }
    }
}
