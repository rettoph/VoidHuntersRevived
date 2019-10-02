using GalacticFighters.Library.Entities.ShipParts.Thrusters;
using Guppy;
using Guppy.Attributes;
using Guppy.Loaders;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Client.Library.Drivers.Entities.ShipParts
{
    [IsDriver(typeof(Thruster))]
    class ClientThrusterDriver : Driver<Thruster>
    {
        private SpriteBatch _spriteBatch;
        private Texture2D _thrust;


        public ClientThrusterDriver(SpriteBatch spriteBatch, ContentLoader content, Thruster driven) : base(driven)
        {
            _spriteBatch = spriteBatch;
            _thrust = content.TryGet<Texture2D>("thrust");
        }

        protected override void Draw(GameTime gameTime)
        {
            if (this.driven.Root.IsBridge && this.driven.GetActive(this.driven.Root.BridgeFor.ActiveDirections))
                _spriteBatch.Draw(
                    texture: _thrust,
                    position: this.driven.WorldCenteroid,
                    sourceRectangle: _thrust.Bounds,
                    color: Color.White,
                    rotation: this.driven.Rotation + MathHelper.Pi,
                    origin: new Vector2(0, _thrust.Bounds.Center.ToVector2().Y),
                    scale: 0.01f,
                    effects: SpriteEffects.None,
                    layerDepth: 0);
        }
    }
}
