﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Extensions;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Providers;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Entities.ConnectionNodes
{
    public class ConnectionNode : Entity
    {
        public Vector2 LocalPoint { get; protected set; }
        public Single LocalRotation { get; protected set; }

        private SpriteBatch _spriteBatch;
        private Texture2D _texture;
        private Vector2 _origin;

        private ShipPart _owner;

        #region Constructors
        public ConnectionNode(String textureName, Vector3 connectionData, ShipPart owner, EntityInfo info, IServiceProvider provider, IGame game, SpriteBatch spriteBatch = null)
            : base(info, game)
        {
            if (spriteBatch != null)
            {
                var contentLoader = provider.GetLoader<ContentLoader>();
                _texture = contentLoader.Get<Texture2D>(textureName);
                _spriteBatch = spriteBatch;
                //_spriteBatch = provider.GetService(typeof(SpriteBatch)) as SpriteBatch;

                // Calculate the centerpoint of the connection node texture
                _origin = _texture == null ? Vector2.Zero : new Vector2(_texture.Bounds.Width / 2, _texture.Bounds.Height / 2);

                this.DrawOrder = 10;
                this.Visible = true;
            }

            // Run the general constructor
            this.Construct(connectionData, owner);
        }
        private void Construct(Vector3 connectionData, ShipPart owner)
        {
            this.LocalPoint = new Vector2(connectionData.X, connectionData.Y);
            this.LocalRotation = connectionData.Z;

            _owner = owner;

            this.Enabled = false;
        }
        #endregion

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Draw(
                texture: _texture,
                position: _owner.Body.Position + Vector2.Transform(this.LocalPoint, _owner.RotationMatrix),
                sourceRectangle: _texture.Bounds,
                color: Color.White,
                rotation: _owner.Body.Rotation + this.LocalRotation,
                origin: _origin,
                scale: 0.01f,
                effects: SpriteEffects.None,
                layerDepth: 0);
        }
    }
}
