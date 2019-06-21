using Guppy;
using Guppy.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Utilities.ConnectionNodes
{
    public abstract class ConnectionNode
    {
        private SpriteBatch _spriteBatch;

        protected Texture2D texture;

        public readonly Int32 Id;

        /// <summary>
        /// The connection node the current node is bound to
        /// (if any)
        /// </summary>
        public ConnectionNode Target { get; protected internal set; }

        /// <summary>
        /// The current nodes parent
        /// </summary>
        public readonly ShipPart Parent;

        #region Position Attributes
        public readonly Single LocalRotation;
        public readonly Vector2 LocalPosition;

        public Vector2 WorldPosition
        {
            get
            {
                return this.Parent.Position + Vector2.Transform(this.LocalPosition, this.OffsetMatrix);
            }
        }
        public Single WorldRotation
        {
            get
            {
                return this.Parent.Rotation + this.LocalRotation;
            }
        }
        public Matrix OffsetMatrix
        {
            get
            {
                return Matrix.CreateRotationZ(this.Parent.Rotation);
            }
        }
        #endregion

        #region Constructors
        public ConnectionNode(Int32 id, ShipPart parent, Single rotation, Vector2 position, SpriteBatch spriteBatch = null)
        {
            _spriteBatch = spriteBatch;

            this.Parent = parent;

            this.LocalRotation = rotation;
            this.LocalPosition = position;
        }
        #endregion

        public virtual void Draw(GameTime gameTime)
        {
            _spriteBatch.Draw(
                texture: this.texture,
                position: this.WorldPosition,
                sourceRectangle: this.texture.Bounds,
                color: Color.White,
                rotation: this.WorldRotation,
                origin: this.texture.Bounds.Center.ToVector2(),
                scale: 0.01f,
                effects: SpriteEffects.None,
                layerDepth: 0);
        }
    }
}
