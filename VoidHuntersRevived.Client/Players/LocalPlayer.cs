using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using VoidHuntersRevived.Client.Utilities.Cameras;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Players;

namespace VoidHuntersRevived.Client.Players
{
    public class LocalPlayer : Player
    {
        private FarseerCamera2D _camera;

        public LocalPlayer(FarseerCamera2D camera, ShipPart bridge, ILogger logger) : base(bridge, logger)
        {
            _camera = camera;
        }

        public override void Update(GameTime gameTime)
        {
            Single speed = 0.1f;

            if(Keyboard.GetState().IsKeyDown(Keys.W))
                this.Bridge.Body.ApplyLinearImpulse(new Vector2(0, -speed));
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                this.Bridge.Body.ApplyLinearImpulse(new Vector2(-speed, 0));
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                this.Bridge.Body.ApplyLinearImpulse(new Vector2(0, speed));
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                this.Bridge.Body.ApplyLinearImpulse(new Vector2(speed, 0));

            _camera.MoveTo(this.Bridge.Body.Position);
        }
    }
}
