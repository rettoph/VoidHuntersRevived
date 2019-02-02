using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Providers;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Core.Extensions;
using VoidHuntersRevived.Library.Entities.Connections;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Entities.ConnectionNodes
{
    /// <summary>
    /// The base ConnectionNode, extended by both the Male and Female
    /// classes. Handles relative and local positions/rotations of each node
    /// </summary>
    public abstract class ConnectionNode : Entity
    {
        #region Private Fields
        private SpriteBatch _spriteBatch;
        private Texture2D _texture;
        private Vector2 _origin;
        #endregion

        #region Public Attributes
        /// <summary>
        /// The ConnectionNode's ShipPart owner
        /// </summary>
        public ShipPart Owner { get; private set; }

        /// <summary>
        /// The ConnectionNode's position relative to its ShipPart owner
        /// </summary>
        public Vector2 LocalPoint { get; protected set; }

        /// <summary>
        /// The ConnectionNode's rotation relative to its ShipPart owner
        /// </summary>
        public Single LocalRotation { get; protected set; }

        /// <summary>
        /// The ConnectionNode's translation matrix realtive to its ShipPart owner
        /// </summary>
        public Matrix LocalTranslationMatrix { get; private set; }

        /// <summary>
        /// The ConnectionNode position realtive to the Farseer World
        /// </summary>
        public Vector2 WorldPoint { get { return this.Owner.Root.Body.Position + Vector2.Transform(this.LocalPoint, this.Owner.OffsetTranslationMatrix * this.Owner.Root.RotationMatrix); } }

        /// <summary>
        /// The ConnectionNode rotation relative to the Farseer World
        /// </summary>
        public Single WorldRotation { get { return this.LocalRotation + this.Owner.Root.Body.Rotation + this.Owner.OffsetRotation; } }

        /// <summary>
        /// The current nodes connection (if any)
        /// </summary>
        public NodeConnection Connection { get; private set; }
        #endregion

        #region Events
        event EventHandler<NodeConnection> OnConnected;
        event EventHandler<ConnectionNode> OnDisconnected;
        #endregion

        #region Constructors
        public ConnectionNode(
            String textureName, 
            ShipPart owner, 
            Vector3 positionData, 
            EntityInfo info, 
            IGame game,
            SpriteBatch spriteBatch = null) : base(info, game)
        {
            this.Owner = owner;

            this.LocalPoint    = new Vector2(positionData.X, positionData.Y);
            this.LocalRotation = positionData.Z;
            this.LocalTranslationMatrix = Matrix.CreateRotationZ(this.LocalRotation)
                * Matrix.CreateTranslation(this.LocalPoint.X, this.LocalPoint.Y, 0);

            if (spriteBatch != null)
            { // Only do any texture related stuff if there is a SpriteBatch...
                var contentLoader = game.Provider.GetLoader<ContentLoader>();
                _texture = contentLoader.Get<Texture2D>(textureName);
                _spriteBatch = spriteBatch; // Save the input SpriteBatch

                // Calculate the centerpoint of the connection node texture
                _origin = _texture == null ? Vector2.Zero : new Vector2(_texture.Bounds.Width / 2, _texture.Bounds.Height / 2);
            }

            this.SetEnabled(false);
            this.SetVisible(true);
        }
        #endregion

        /// <summary>
        /// Attempt to connect to a given NodeConnection
        /// </summary>
        /// <param name="connection"></param>
        internal void Connect(NodeConnection connection)
        {
            if (connection.Status != ConnectionStatus.Connecting)
                throw new Exception("Unable to bind ConnectionNode to requested connection. Invalid Connection Status.");
            else if(this.Connection != null)
                throw new Exception("Unable to bind ConnectionNode to requested connection. Node already bound to a connection.");
            else if(connection.FemaleConnectionNode != this && connection.MaleConnectionNode != this)
                throw new Exception("Unable to bind ConnectionNode to requested connection. Irrelevant connection.");

            // Save the new connection
            this.Connection = connection;

            // Update the owners offset translation data
            this.Owner.UpdateTransformationData();

            // Invoke the OnConnected event
            this.OnConnected?.Invoke(this, connection);
        }

        /// <summary>
        /// Attempt to disconnect from the current NodeConnection
        /// </summary>
        internal void Disconnect()
        {
            if (this.Connection == null)
                throw new Exception("Unable to un-bind ConnectionNode from current connection. No current connection.");
            else if (this.Connection.Status != ConnectionStatus.Disconnecting)
                throw new Exception("Unable to un-bind ConnectionNode from current connection. Invalid Connection Status.");

            // Discard the old connection
            this.Connection = null;

            // Invoke the OnDisconnected event
            this.OnDisconnected?.Invoke(this, this);
        }

        #region Frame Methods
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
        #endregion
    }
}
