using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Providers;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.ConnectionNodes;
using VoidHuntersRevived.Library.Entities.Drivers;
using VoidHuntersRevived.Library.Entities.MetaData;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Networking.Implementations;
using VoidHuntersRevived.Core.Extensions;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    /// <summary>
    /// A ShipPart represents the base class all ship related entities extend from.
    /// By default, ship parts are all grabbable by a tractor beam, have a single
    /// male connection node, and can have multiple female connection nodes 
    /// </summary>
    public partial class ShipPart : NetworkEntity
    {
        #region Private Fields
        private String _driverHandle;
        private Driver _driver;

        private SpriteBatch _spriteBatch;
        private Texture2D _centerOfMass;
        private Vector2 _centerOfMassOrigin;
        #endregion

        #region Protected Fields
        protected MainGameScene _scene;
        #endregion

        #region Public Attributes
        /// <summary>
        /// The current ShipPart's data. This contains info such as connection node locations, and
        /// base shap vertices
        /// </summary>
        public ShipPartData Data { get; private set; }

        /// <summary>
        /// The ShipPart's rootmost ShipPart. If there is no connetion, this will return the current ship part.
        /// </summary>
        public ShipPart Root { get { return this.MaleConnectionNode.Connection == null ? this : this.MaleConnectionNode.Connection.FemaleConnectionNode.Owner.Root; } }

        /// <summary>
        /// The ShipPart's immediate parent. If there is no connection, this will return null.
        /// </summary>
        public ShipPart Parent { get { return this.MaleConnectionNode.Connection == null ? null : this.MaleConnectionNode.Connection.FemaleConnectionNode.Owner; } }

        public Boolean IsRoot { get { return this.Root == this; } }
        public Boolean HasParent { get { return this.Parent != null; } }

        /// <summary>
        /// The ShipPart's current Farseer Fixture. Note, this might not reside within the ShipPart's Body
        /// (In the instance of a ShipPart being attatched to another ShipPart)
        /// </summary>
        public Fixture Fixture { get; private set; }

        /// <summary>
        /// The ShipPart's main Farseer Body. Note, this body is usually only in use if the ShipPart is the
        /// chains root, or if the ShipPart uses a join to attatch to another ShipPart
        /// (Like rotating weapons)
        /// </summary>
        public Body Body { get; private set; }

        /// <summary>
        /// The ShipPart's MaleConnectionNode
        /// </summary>
        public MaleConnectionNode MaleConnectionNode { get; private set; }

        /// <summary>
        /// The ShipPart's FemaleConnectionNodes
        /// </summary>
        public FemaleConnectionNode[] FemaleConnectionNodes { get; private set; }
        #endregion

        #region Constructors
        public ShipPart(EntityInfo info, IGame game, String driverHandle = "entity:driver:ship_part") : base(info, game)
        {
            _driverHandle = driverHandle;

            this.Data = this.Info.Data as ShipPartData;
        }

        public ShipPart(long id, EntityInfo info, IGame game, SpriteBatch spriteBatch, String driverHandle = "entity:driver:ship_part") : base(id, info, game)
        {
            _driverHandle = driverHandle;
            _spriteBatch = spriteBatch;

            // Load the center of mass texture
            var contentLoader = game.Provider.GetLoader<ContentLoader>();
            _centerOfMass = contentLoader.Get<Texture2D>("texture:center_of_mass");
            _centerOfMassOrigin = new Vector2((float)_centerOfMass.Width / 2, (float)_centerOfMass.Height / 2);

            this.Data = this.Info.Data as ShipPartData;
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            // Store the ShipPart's main game scene
            _scene = this.Scene as MainGameScene;

            // Create a new Body and Fixture for the current ship part
            this.Body = this.CreateBody();
            
            // Create a new Driver for the current ShipPart
            _driver = this.Scene.Entities.Create<Driver>(_driverHandle, null, this);

            // Create the ShipPart's MaleConnectionNode
            this.MaleConnectionNode = this.Scene.Entities.Create<MaleConnectionNode>("entity:connection_node:male", this.Layer, this, this.Data.MaleConnectionNodeData);

            // Create the ShipPart's FemaleConnectionNodes
            this.FemaleConnectionNodes = this.Data.FemaleConnectionNodesData
                .Select(fcnd =>
                {
                    return this.Scene.Entities.Create<FemaleConnectionNode>("entity:connection_node:female", this.Layer, this, fcnd);
                })
                .ToArray();

            // Update the initial transformation data
            this.UpdateTransformationData();

            // Update the current chain placement
            this.UpdateChainPlacement();

            // Add ShipPart event handlers
            this.MaleConnectionNode.OnConnected += this.HandleMaleConnectionNodeConnected;
            this.MaleConnectionNode.OnDisconnected += this.HandleMaleConnectionNodeDisonnected;
        }
        #endregion

        #region Methods
        /// <summary>
        /// When a ShipPart gets added to another, certain actions must
        /// take place. By default, a ShipPart will create a new fixture
        /// with proper translations and add it to the Root most
        /// ship part.
        /// 
        /// Other ShipParts may override this functionality, however. 
        /// Such as weapons, which will use a ROtationJoint for attatcment
        /// rather than a fixture transplant
        /// </summary>
        protected virtual void UpdateChainPlacement()
        {
            // Delete the old fixture, if it existed
            this.Fixture?.Body.DestroyFixture(this.Fixture);

            // Create a new fixture on the root body
            this.Fixture = this.CreateFixture(this.Root.Body, this.OffsetTranslationMatrix);

            
            if (this.IsRoot)
            { // Root only updates here:
                // When a chain gets updates, the root must update its center of mass
                this.Body.ResetMassData();
            }
            else
            { // Children updates here:
                // First, ensure the transformation data is up to date
                this.UpdateTransformationData();

                // Update the current root data
                this.Root.UpdateChainPlacement();

                // Update all the current ShipPart's children as well
                foreach (FemaleConnectionNode femaleConnectionNode in this.FemaleConnectionNodes)
                    femaleConnectionNode.Connection?.MaleConnectionNode.Owner.UpdateChainPlacement();
            }
        }
        #endregion

        #region Frame Methods
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update the transformation data
            this.UpdateTransformationData();

            // Update the driver..
            _driver.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if(this.IsRoot)
            {
                _spriteBatch.Draw(
                    texture: _centerOfMass,
                    position: this.Body.Position + Vector2.Transform(this.Body.LocalCenter, this.RotationMatrix),
                    sourceRectangle: _centerOfMass.Bounds,
                    color: Color.White,
                    rotation: this.Body.Rotation,
                    origin: _centerOfMassOrigin,
                    scale: 0.01f,
                    effects: SpriteEffects.None,
                    layerDepth: 0);
            }
        }
        #endregion

        #region INetworkEntity Implementation
        /// <inheritdoc />
        public override void Read(NetIncomingMessage im)
        {
            if(im.ReadBoolean()) // Only read to the driver if the confirmation byte was recieved
                _driver.Read(im);
        }

        /// <inheritdoc />
        public override void Write(NetOutgoingMessage om)
        {
            // Write the current id
            om.Write(this.Id);

            if (_driver == null) // No driver to send, so so data to send
                om.Write(false);
            else
            { // Send the driver confirmation byte, then send the driver data
                om.Write(true);
                _driver.Write(om);
            }
        }
        #endregion
    }
}
