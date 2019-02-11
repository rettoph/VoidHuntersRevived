using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Library.Entities.MetaData
{
    public class ThrusterData : ShipPartData
    {
        public readonly Single Acceleration;

        public ThrusterData(
            Vector2 maleConnectionNodeData,
            Vector2[] vertices,
            Single acceleration,
            String textureHandle,
            Vector2 textureOrigin,
            String colorHandle) : base(maleConnectionNodeData, vertices, new Vector3[0], textureHandle, textureOrigin, colorHandle)
        {
            this.Acceleration = acceleration;
        }
    }
}
