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
        /// Whether or not the current node is connected to another node
        /// </summary>
        public Boolean Connected { get { return this.Target != null; } }

        /// <summary>
        /// The current nodes parent
        /// </summary>
        public readonly ShipPart Parent;

        #region Position Attributes
        public readonly Single LocalRotation;
        public readonly Vector2 LocalPosition;

        public readonly Matrix LocalRotationMatrix;
        public readonly Matrix LocalTranslationMatrix;
        public readonly Matrix LocalTransformationMatrix;

        public Vector2 WorldPosition
        {
            get
            {
                return this.Parent.Root.Position + Vector2.Transform(this.LocalPosition, this.Parent.LocalTransformation * Matrix.CreateRotationZ(this.Parent.Root.Rotation));
            }
        }
        public Single WorldRotation
        {
            get
            {
                return this.Parent.Root.Rotation + this.Parent.LocalRotation + this.LocalRotation;
            }
        }
        #endregion

        #region Constructors
        public ConnectionNode(Int32 id, ShipPart parent, Single rotation, Vector2 position, SpriteBatch spriteBatch = null)
        {
            _spriteBatch = spriteBatch;

            this.Id = id;

            this.Parent = parent;

            this.LocalRotation = rotation;
            this.LocalPosition = position;

            this.LocalRotationMatrix = Matrix.CreateRotationZ(this.LocalRotation);
            this.LocalTranslationMatrix = Matrix.CreateTranslation(this.LocalPosition.X, this.LocalPosition.Y, 0);

            // A onestep transformation, first move to rotation position then translate by position offset.
            this.LocalTransformationMatrix = this.LocalRotationMatrix * this.LocalTranslationMatrix;
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
