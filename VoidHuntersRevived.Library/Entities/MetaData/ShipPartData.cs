using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Entities.MetaData
{
    /// <summary>
    /// Contains general information about a ship part.
    /// This should only be used within the game initializer 
    /// when registering a new ship part. Use an instance of
    /// the ShipPartData class for the custom data attribute
    /// within the EntityLoader
    /// </summary>
    public class ShipPartData
    {
        public readonly Vector3 MaleConnectionNodeData;
        public readonly Vector2[] Vertices;
        public readonly Vector3[] FemaleConnectionNodesData;

        public ShipPartData(Vector3 maleConnectionNodeData, Vector2[] vertices, Vector3[] femaleConnectionNodesData)
        {
            this.MaleConnectionNodeData = maleConnectionNodeData;
            this.Vertices = vertices;
            this.FemaleConnectionNodesData = femaleConnectionNodesData;
        }
    }
}
