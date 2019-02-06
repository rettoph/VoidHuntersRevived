using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Providers;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Interfaces;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Core.Extensions;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.Connections;
using VoidHuntersRevived.Library.Scenes;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Contacts;
using System.Linq;
using VoidHuntersRevived.Library.Enums;
using Microsoft.Extensions.Logging;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// Each player instance will contain a tractor beam.
    /// The player itself should manage the tractor beam functionality
    /// </summary>
    public class TractorBeam : Entity, IFarseerEntity
    {
        #region Static Fields
        /// <summary>
        /// The distance the TractorBeam can be from an available FemaleConnectionNode
        /// when deselecting an entity in order for a new NodeConnection to be created.
        /// </summary>
        public static Single AttachmentDistance = 1;
        #endregion

        #region Private Fields
        private SpriteBatch _spriteBatch;

        private Texture2D _texture;
        private Vector2 _origin; // The Texture2D's origin

        private List<ShipPart> _contacts; // List of shipparts the tractor beam is currently over
        #endregion

        #region Public Attributes
        /// <summary>
        /// The Player the current TractorBeam belongs to
        /// </summary>
        public Player Player { get; private set; }

        /// <summary>
        /// The TractorBeam's current connection
        /// </summary>
        public TractorBeamConnection Connection { get; private set; }

        /// <summary>
        /// Represents the current TractorBeam's grab radius
        /// </summary>
        public Body Body { get; private set; }
        #endregion

        #region Events
        public event EventHandler<TractorBeamConnection> OnConnected;
        public event EventHandler<TractorBeam> OnDisconnected;
        #endregion

        #region Constructors
        public TractorBeam(Player player, EntityInfo info, IGame game, SpriteBatch spriteBatch = null) : base(info, game)
        {
            // Save the incoming player
            this.Player = player;
            // Save the incoming spritebatch
            _spriteBatch = spriteBatch;
            // Create an empty contacts container
            _contacts = new List<ShipPart>();
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            var scene = this.Scene as MainGameScene;

            // Create a new body for the TractorBeam
            this.Body = BodyFactory.CreateCircle(
                scene.World,
                1f,
                0f,
                Vector2.Zero,
                BodyType.Dynamic,
                this);
            this.Body.IsSensor = true;
            this.Body.SleepingAllowed = false;

            if(_spriteBatch != null)
            { // Only bother loading content data if there is a spritebatch
                var contentLoader = this.Game.Provider.GetLoader<ContentLoader>();
                // Load the tractor beam texture
                _texture = contentLoader.Get<Texture2D>("texture:tractor_beam");
                // Define the tractor beam origin
                _origin = new Vector2((float)_texture.Bounds.Width / 2, (float)_texture.Bounds.Height / 2);
                // Ensure that the tractor beam is visible
                this.SetVisible(true);
            }

            // Add event listeners
            this.Body.OnCollision += this.HandleCollision;
            this.Body.OnSeparation += this.HandleSeperation;
        }
        #endregion

        #region Connection Methods
        /// <summary>
        /// Attempt to select the current  hovered ShipPart
        /// </summary>
        public void Select()
        {
            //Selected the current hovered over ShipPart
            ShipPart hovered = _contacts
                .OrderBy(sp => Vector2.Distance(
                    this.Body.Position, 
                    sp.Root.Body.Position + Vector2.Transform(
                        Vector2.Zero, sp.OffsetTranslationMatrix * sp.Root.RotationMatrix)))
                .FirstOrDefault();

            if (hovered != null && !hovered.IsBridge)
            { // Only bother trying if there is a valid target...
                // If the hovered is part of a free floating chain, grab the root most ShipPart
                ShipPart target = hovered.Root.IsBridge ? hovered : hovered.Root;

                if (target.Root.IsBridge && this.Player != target.Root.BridgeFor)
                    this.Game.Logger.LogError("Requested TractorBeam target belongs to another player!");
                else if (this.Connection != null)
                    this.Game.Logger.LogError("TractorBeam already contains another TractorBeamConnection!");
                else if (target.Connection != null)
                    this.Game.Logger.LogError("ShipPart target already contains another TractorBeamConnection!");
                else
                { // Now that we have been validated...
                    // Destroy any pre-existing male connection on the target, if any
                    target.MaleConnectionNode.Connection?.Disconnect();

                    // Create a new TractorBeamConnection...
                    this.Scene.Entities.Create<TractorBeamConnection>("entity:connection:tractor_beam", null, this, target);
                }
            }
        }

        /// <summary>
        /// Attempt to connect to a given TractorBeamConnection
        /// </summary>
        /// <param name="connection"></param>
        internal virtual void Connect(TractorBeamConnection connection)
        {
            if (connection.Status != ConnectionStatus.Connecting)
                throw new Exception("Unable to bind TractorBeam to requested connection. Invalid Connection Status.");
            else if (this.Connection != null)
                throw new Exception("Unable to bind TractorBeam to requested connection. TractorBeam already bound to a connection.");
            else if (connection.TractorBeam != this)
                throw new Exception("Unable to bind TractorBeam to requested connection. Irrelevant connection.");

            // Save the new connection
            this.Connection = connection;

            // Invoke the OnConnected event
            this.OnConnected?.Invoke(this, connection);
        }

        /// <summary>
        /// Attempt to disconnect from the current NodeConnection
        /// </summary>
        internal virtual void Disconnect()
        {
            if (this.Connection == null)
                throw new Exception("Unable to un-bind TractorBeam from current connection. No current connection.");
            else if (this.Connection.Status != ConnectionStatus.Disconnecting)
                throw new Exception("Unable to un-bind TractorBeam from current connection. Invalid Connection Status.");

            // Discard the old connection
            this.Connection = null;

            // Invoke the OnDisconnected event
            this.OnDisconnected?.Invoke(this, this);
        }
        #endregion

        #region Frame Methods
        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Draw(
                texture: _texture,
                position: this.Body.Position,
                sourceRectangle: _texture.Bounds,
                color: Color.White,
                rotation: 0,
                origin: _origin,
                scale: 0.01f,
                effects: SpriteEffects.None,
                layerDepth: 0);
        }
        #endregion

        #region Event Handlers
        private bool HandleCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (fixtureB.UserData is ShipPart)
                _contacts.Add(fixtureB.UserData as ShipPart);

            return true;
        }

        private void HandleSeperation(Fixture fixtureA, Fixture fixtureB)
        {
            _contacts.Remove(fixtureB.UserData as ShipPart);
        }
        #endregion
    }
}
