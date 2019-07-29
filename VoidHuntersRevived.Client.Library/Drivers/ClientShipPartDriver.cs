using Guppy.Collections;
using Guppy.Implementations;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using System.Linq;
using Guppy.Loaders;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Client.Library.Drivers
{
    public class ClientShipPartDriver : Driver
    {
        #region Private Fields
        private ShipPart _shipPart;
        private EntityCollection _entities;
        private Texture2D _com;
        private SpriteBatch _spriteBatch;
        #endregion

        #region Constructors
        public ClientShipPartDriver(ShipPart parent, ContentLoader content, SpriteBatch spriteBatch, EntityCollection entities, IServiceProvider provider) : base(parent, provider)
        {
            _shipPart = parent;
            _entities = entities;
            _spriteBatch = spriteBatch;
            _com = content.Get<Texture2D>("texture:com");
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            _shipPart.Actions.AddHandler("male-connection-node:detach", this.HandleMaleConnectionNodeDetachAction);
            _shipPart.Actions.AddHandler("male-connection-node:attach", this.HandleMaleConnectionNodeAttachAction);
        }
        #endregion

        #region Frame Methods
        protected override void draw(GameTime gameTime)
        {
            base.draw(gameTime);

            // _spriteBatch.Draw(
            //     texture: _com,
            //     position: _shipPart.WorldCenteroid,
            //     sourceRectangle: _com.Bounds,
            //     color: Color.White,
            //     rotation: _shipPart.Rotation,
            //     origin: _com.Bounds.Center.ToVector2(),
            //     scale: 0.01f,
            //     effects: SpriteEffects.None,
            //     layerDepth: 0);
        }
        #endregion

        #region Action Handlers
        private void HandleMaleConnectionNodeDetachAction(Object sender, NetIncomingMessage obj)
        {
            // Only detatch if the ship part is connected to anything
            if (_shipPart.MaleConnectionNode.Target != null)
                _shipPart.TryDetatchFrom();
        }

        private void HandleMaleConnectionNodeAttachAction(Object sender, NetIncomingMessage obj)
        {
            _shipPart.ReadAttachmentData(obj);
        }
        #endregion
    }
}
