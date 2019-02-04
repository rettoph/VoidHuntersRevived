using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.ConnectionNodes;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Networking.Implementations;

namespace VoidHuntersRevived.Library.Entities.Connections
{
    /// <summary>
    /// Represents a link between 2 ConnectionNodes.
    /// One female and the other male. The creation 
    /// of a NodeConnection must be validated in several
    /// ways. Primarily, each ConnectionNode cannot already
    /// have another connection.
    /// 
    /// If there is an error in any way during creation, this
    /// will throw an exception...
    /// </summary>
    public class NodeConnection : NetworkEntity
    {
        #region Public Attributes
        public MaleConnectionNode MaleConnectionNode { get; private set; }
        public FemaleConnectionNode FemaleConnectionNode { get; private set; }

        public ConnectionStatus Status { get; private set; }
        #endregion

        #region Constructors
        public NodeConnection(
            MaleConnectionNode maleConnectionNode,
            FemaleConnectionNode femaleConnectionNode,
            EntityInfo info,
            IGame game) : base(info, game)
        {
            this.Status = ConnectionStatus.Initializing;

            this.MaleConnectionNode = maleConnectionNode;
            this.FemaleConnectionNode = femaleConnectionNode;

            // Complete the connection
            this.ApplyConnection();
        }

        public NodeConnection(
            long id,
            EntityInfo info, 
            IGame game) : base(id, info, game)
        {
            this.Status = ConnectionStatus.Initializing;
        }
        #endregion

        /// <summary>
        /// Once all the connection info required has been recieved we can alert
        /// the MaleConnectionNode and FemaleConnectionNode of the connections creation. 
        /// The following method validates the input MaleConnectionNode and 
        /// FemaleConnectionNode, mark them as connected, and complete and additional 
        /// setup as needed
        /// </summary>
        private void ApplyConnection()
        {
            // Update the current connection status
            this.Status = ConnectionStatus.Connecting;

            if (this.MaleConnectionNode.Owner == this.FemaleConnectionNode.Owner)
                throw new Exception("Unable to apply NodeConnection. MaleConnectionNode and FemaleConnectionNode have the same owner.");

            // Mark the ConnectionNodes as connected
            this.MaleConnectionNode.Connect(this);
            this.FemaleConnectionNode.Connect(this);

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

            // Mark the ConnectionNodes as disconnected
            this.MaleConnectionNode.Disconnect();
            this.FemaleConnectionNode.Disconnect();

            // Delete the current connection entity from the scene
            this.Delete();

            // Update the current connection status
            this.Status = ConnectionStatus.Disconnecting;
        }

        public override bool Delete()
        {
            if(base.Delete())
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
                throw new Exception("Unable to read incoming NodeConnection data. Invalid Connection Status.");

            // Temp load the current NodeConnection's scene, to select the relevant ShipPart's ConnectionNodes
            var scene = this.Scene as MainGameScene;

            // Read the MaleConnectionNode...
            this.MaleConnectionNode = (scene.NetworkEntities.GetById(im.ReadInt64()) as ShipPart).MaleConnectionNode;

            // Read the FemaleConnectionNode...
            this.FemaleConnectionNode = (scene.NetworkEntities.GetById(im.ReadInt64()) as ShipPart).FemaleConnectionNodes[im.ReadInt32()];

            // Now that we have the required ConnectionNode data we can apply the connection
            this.ApplyConnection();
        }

        public override void Write(NetOutgoingMessage om)
        {
            om.Write(this.Id);

            // Write the required MaleConnectionNode data
            om.Write(this.MaleConnectionNode.Owner.Id);

            // Write the required FemaleConnectionNode data
            om.Write(this.FemaleConnectionNode.Owner.Id);
            om.Write(Array.IndexOf(this.FemaleConnectionNode.Owner.FemaleConnectionNodes, this.FemaleConnectionNode));
        }
        #endregion
    }
}
