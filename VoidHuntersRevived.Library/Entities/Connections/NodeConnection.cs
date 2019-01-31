using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Connections.Nodes;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Entities.Connections
{
    /// <summary>
    /// A connection represents a linkage between a female node and
    /// a male node
    /// </summary>
    public class NodeConnection : Entity
    {
        public ConnectionState State;

        public readonly FemaleConnectionNode FemaleNode;
        public readonly MaleConnectionNode MaleNode;

        public Matrix MaleNodeOffset { get; private set; }

        public NodeConnection(FemaleConnectionNode female, MaleConnectionNode male, EntityInfo info, IGame game) : base(info, game)
        {
            this.Enabled = false;
            this.Visible = false;

            this.State = ConnectionState.Connecting;

            this.FemaleNode = female;
            this.MaleNode = male;

            this.FemaleNode.Connect(this);
            this.MaleNode.Connect(this);

            this.MaleNodeOffset = Matrix.CreateTranslation(new Vector3(0, 0, 0));

            this.State = ConnectionState.Connected;
        }

        // Terminate the connection
        public void Disconnect()
        {
            this.State = ConnectionState.Disconnecting;

            this.MaleNode.Disconnect();
            this.FemaleNode.Disconnect();

            this.State = ConnectionState.Disconnected;

            // Remove the entity
            this.Scene.Entities.Remove(this);
        }
    }
}
