using FarseerPhysics.Collision.Shapes;
using Guppy.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Configurations
{
    public struct ShipPartConfiguration : IEntityData
    {
        public readonly PolygonShape Shape;
        public readonly Vector3 MaleConnectionNode;
        public readonly Vector3[] FemaleConnectionNodes;

        public ShipPartConfiguration(
            PolygonShape shape,
            Vector3 maleConnectionNode,
            Vector3[] femaleConnectionNodes = null)
        {
            this.Shape = shape;
            this.MaleConnectionNode = maleConnectionNode;
            this.FemaleConnectionNodes = femaleConnectionNodes == null ? new Vector3[0] : femaleConnectionNodes;
        }

        public void Initialize(IServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
