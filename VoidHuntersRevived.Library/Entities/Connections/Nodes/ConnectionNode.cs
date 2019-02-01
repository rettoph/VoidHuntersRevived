using Microsoft.Xna.Framework;
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
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Extensions;

namespace VoidHuntersRevived.Library.Entities.Connections.Nodes
{
    public class ConnectionNode : Entity
    {
        /// <summary>
        /// The node position relative to its ship part owner
        /// </summary>
        public Vector2 LocalPoint { get; protected set; }

        /// <summary>
        /// The node rotation relative to its ship part owner
        /// </summary>
        public Single LocalRotation { get; protected set; }

        public Vector2 WorldPoint { get { return this.Owner.Root.Body.Position + Vector2.Transform(this.LocalPoint, (this.Owner == this.Owner.Root ? this.Owner.TransformationOffsetMatrix : this.Owner.TransformationOffsetMatrix * this.Owner.Root.TransformationOffsetMatrix)); } }
        public Single WorldRotation { get { return (this.Owner == this.Owner.Root ? this.LocalRotation + this.Owner.Root.Body.Rotation : this.Owner.RotationOffset.ToAxisAngle().Z + this.LocalRotation + this.Owner.Root.Body.Rotation); } }

        public Matrix TranslationMatrix { get; private set; }

        private SpriteBatch _spriteBatch;
        private Texture2D _texture;
        private Vector2 _origin;

        /// <summary>
        /// The ship part this connection node belongs to
        /// </summary>
        public ShipPart Owner { get; private set; }

        /// <summary>
        /// The nodes current connection (if any)
        /// </summary>
        public NodeConnection Connection { get; protected set; }

        // Public events triggered when the node makes or breaks a connection
        public event EventHandler<ConnectionNode> OnConnected;
        public event EventHandler<ConnectionNode> OnDisconnected;

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

                this.Enabled = false;
            }

            // Run the general constructor
            this.Construct(connectionData, owner);
        }
        private void Construct(Vector3 connectionData, ShipPart owner)
        {
            this.LocalPoint = new Vector2(connectionData.X, connectionData.Y);
            this.LocalRotation = connectionData.Z;

            this.TranslationMatrix = Matrix.CreateRotationZ(this.LocalRotation)
                *  Matrix.CreateTranslation(this.LocalPoint.X, this.LocalPoint.Y, 0);

            Owner = owner;
        }
        #endregion

        #region Connection Interaction Methods
        /// <summary>
        /// Link to a new connection
        /// </summary>
        /// <param name="connection"></param>
        public virtual void Connect(NodeConnection connection)
        {
            // The connection can only connect if its not already connnected
            if (this.Connection != null)
                throw new Exception("Unable to connect! Already bound to another connection.");
            else if (connection.State != ConnectionState.Connecting)
                throw new Exception("Unable to connect! Connection state is not set to Connecting.");
            else if (connection.FemaleNode != this && connection.MaleNode != this)
                throw new Exception("Unable to connect! Incoming connection does not reference current node.");

            // Save the incoming connection
            this.Connection = connection;

            this.Owner.UpdateOffsetFields();

            this.OnConnected?.Invoke(this, this);
        }

        /// <summary>
        /// Unlink the stored value to the connection
        /// </summary>
        public virtual void Disconnect()
        {
            if(this.Connection.State != ConnectionState.Disconnected)
                throw new Exception("Unable to connect! Connection state is not set to Disconnected.");

            this.Connection = null;

            this.OnDisconnected?.Invoke(this, this);
        }
        #endregion

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Draw(
                texture: _texture,
                position: this.WorldPoint,
                sourceRectangle: _texture.Bounds,
                color: Color.White,
                rotation: this.WorldRotation,
                origin: _origin,
                scale: 0.01f,
                effects: SpriteEffects.None,
                layerDepth: 0);
        }
    }
}
