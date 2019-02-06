using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Networking.Implementations;

namespace VoidHuntersRevived.Library.Entities.Connections
{
    /// <summary>
    /// A TractorBeamConnection represents a binding between a
    /// TractorBeam and a ShipPart. It will create an in world 
    /// weld joint to bind the 2 components together.
    /// </summary>
    public class TractorBeamConnection : NetworkEntity
    {
        #region Private Fields
        private WeldJoint _joint;
        private MainGameScene _scene;
        #endregion

        #region Public Attributes
        public ShipPart ShipPart { get; private set; }
        public TractorBeam TractorBeam { get; private set; }

        public ConnectionStatus Status { get; private set; }
        #endregion

        #region Constructors
        public TractorBeamConnection(
            ShipPart shipPart,
            TractorBeam tractorBeam,
            EntityInfo info,
            IGame game) : base(info, game)
        {
            this.Status = ConnectionStatus.Initializing;

            this.ShipPart = shipPart;
            this.TractorBeam = tractorBeam;
        }

        public TractorBeamConnection(
            long id,
            EntityInfo info,
            IGame game) : base(id, info, game)
        {
            this.Status = ConnectionStatus.Initializing;
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            _scene = this.Scene as MainGameScene;

            // Run the connection builder now, if ShipPart and TractorBeam are defined
            if (this.TractorBeam != null && this.ShipPart != null)
                this.ApplyConnection();
        }
        #endregion

        /// <summary>
        /// Once all the connection info required has been recieved we can alert
        /// the ShipPart and TractorBeam of the connections creation. 
        /// The following method validates the input ShipPart and 
        /// TractorBeam, mark them as connected, and complete and additional 
        /// setup as needed
        /// </summary>
        private void ApplyConnection()
        {
            // Update the current connection status
            this.Status = ConnectionStatus.Connecting;

            this.TractorBeam.Connect(this);
            this.ShipPart.Connect(this);

            // Create a new weld join between the TractorBeam and the target
            _joint = JointFactory.CreateWeldJoint(
                _scene.World,
                this.TractorBeam.Body,
                this.ShipPart.Body,
                this.TractorBeam.Body.LocalCenter,
                this.ShipPart.MaleConnectionNode.LocalPoint - Vector2.Transform(new Vector2(0.5f, 0f), Matrix.CreateRotationZ(this.ShipPart.MaleConnectionNode.LocalRotation)));
            // Update the current connection status
            this.Status = ConnectionStatus.Connected;
        }

        /// <summary>
        /// Destroy the current NodeConnection
        /// </summary>
        public void Disconnect()
        {
            // Update the current connection status
            this.Status = ConnectionStatus.Disconnecting;

            this.TractorBeam.Disconnect();
            this.ShipPart.Disconnect();

            // Remove the old weld join
            _scene.World.RemoveJoint(_joint);

            // Delete the current connection entity from the scene
            this.Delete();

            // Update the current connection status
            this.Status = ConnectionStatus.Disconnecting;
        }

        public override bool Delete()
        {
            if (base.Delete())
            {
                if (this.Status == ConnectionStatus.Connected)
                    this.Disconnect();

                return true;
            }

            return false;
        }

        #region INetworkEntity Implemntation
        public override void Read(NetIncomingMessage im)
        {
            if (this.Status != ConnectionStatus.Initializing)
                throw new Exception("Unable to read incoming TractorBeamConnection data. Invalid Connection Status.");

            // Temp load the current NodeConnection's scene, to select the relevant ShipPart's ConnectionNodes
            var scene = this.Scene as MainGameScene;

            // Read the ShipPart
            this.ShipPart = scene.NetworkEntities.GetById(im.ReadInt64()) as ShipPart;
            // Read the TractorBeam
            this.TractorBeam = (scene.NetworkEntities.GetById(im.ReadInt64()) as Player).TractorBeam;

            // Now that we have the required data we can apply the connection
            this.ApplyConnection();
        }

        public override void Write(NetOutgoingMessage om)
        {
            om.Write(this.Id);

            om.Write(this.ShipPart.Id);
            om.Write(this.TractorBeam.Player.Id);
        }
        #endregion
    }
}
