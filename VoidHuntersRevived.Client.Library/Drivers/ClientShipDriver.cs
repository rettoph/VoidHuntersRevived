using Guppy.Collections;
using Guppy.Implementations;
using Guppy.Loaders;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Client.Library.Drivers
{
    public class ClientShipDriver : Driver
    {
        #region Private Fields
        private Ship _ship;
        private SpriteBatch _spriteBatch;
        private Texture2D _com;
        #endregion

        #region Constructors
        public ClientShipDriver(Ship parent, ContentLoader content, SpriteBatch spriteBatch, IServiceProvider provider) : base(parent, provider)
        {
            _ship = parent;
            _spriteBatch = spriteBatch;
            _com = content.Get<Texture2D>("texture:com:ship");
        }
        #endregion

        protected override void draw(GameTime gameTime)
        {
            base.draw(gameTime);

            if(_ship.Bridge != null)
                _spriteBatch.Draw(
                    texture: _com,
                    position: _ship.Bridge.WorldCenter,
                    sourceRectangle: _com.Bounds,
                    color: Color.White,
                    rotation: _ship.Bridge.Rotation,
                    origin: _com.Bounds.Center.ToVector2(),
                    scale: 0.01f,
                    effects: SpriteEffects.None,
                    layerDepth: 0);
        }

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            _ship.AddActionHandler("set:bridge", this.HandleSetBridgeAction);
        }
        #endregion

        #region Action Handlers
        /// <summary>
        /// When the ship recieves a set bridge data we must
        /// extract the data from the incoming message then
        /// set the bridge value
        /// </summary>
        /// <param name="obj"></param>
        private void HandleSetBridgeAction(NetIncomingMessage obj)
        {
            _ship.ReadBridgeData(obj);
        }
        #endregion

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
