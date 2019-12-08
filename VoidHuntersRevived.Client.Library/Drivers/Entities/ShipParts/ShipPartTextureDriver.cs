using Guppy;
using Guppy.Attributes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Library.Configurations;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities.ShipParts
{
    /// <summary>
    /// Auto render all ship parts
    /// </summary>
    [IsDriver(typeof(ShipPart))]
    internal sealed class ShipPartTextureDriver : Driver<ShipPart>
    {
        #region Private Fields
        private SpriteManager _sprite;
        #endregion

        #region Constructor
        public ShipPartTextureDriver(SpriteManager sprite, ShipPart driven) : base(driven)
        {
            _sprite = sprite;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            // Load the sprite manager content...
            _sprite.Load(
                texture: $"texture:{this.driven.Configuration.Handle}", 
                vertices: this.driven.Configuration.GetData<ShipPartConfiguration>().Vertices);
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _sprite.Draw(
                position: this.driven.Position, 
                rotation: this.driven.Rotation, 
                color: new Color(Color.Lerp(Color.Red, this.driven.Color, this.driven.Health / 100), 200));
        }
        #endregion
    }
}
