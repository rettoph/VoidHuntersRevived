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
            Single acceleration) : base(maleConnectionNodeData, vertices, new Vector3[0])
        {
            this.Acceleration = acceleration;
        }
    }
}
