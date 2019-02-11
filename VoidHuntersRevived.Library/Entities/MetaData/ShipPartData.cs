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
        public readonly String TextureHandle;
        public readonly Vector2 TextureOrigin;
        public readonly String ColorHandle;

        public readonly Vector2 MaleConnectionNodeData;
        public readonly Vector2[] Vertices;
        public readonly Vector3[] FemaleConnectionNodesData;

        public ShipPartData(
            Vector2 maleConnectionNodeData,
            Vector2[] vertices,
            Vector3[] femaleConnectionNodesData,
            String textureHandle,
            Vector2 textureOrigin,
            String colorHandle)
        {
            this.MaleConnectionNodeData = maleConnectionNodeData;
            this.Vertices = vertices;
            this.FemaleConnectionNodesData = femaleConnectionNodesData;
            this.TextureHandle = textureHandle;
            this.TextureOrigin = textureOrigin;
            this.ColorHandle = colorHandle;
        }
    }
}
