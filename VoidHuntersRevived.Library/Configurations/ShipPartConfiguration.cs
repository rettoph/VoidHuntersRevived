using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Guppy.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Configurations
{
    public struct ShipPartConfiguration : IEntityData
    {
        public readonly Vertices Vertices;
        public readonly Vector3 MaleConnectionNode;
        public readonly Vector3[] FemaleConnectionNodes;
        public readonly Vector2 Centeroid;

        public ShipPartConfiguration(
            Vertices vertices,
            Vector3 maleConnectionNode,
            Vector3[] femaleConnectionNodes = null)
        {
            this.Vertices = vertices;
            this.MaleConnectionNode = maleConnectionNode;
            this.FemaleConnectionNodes = femaleConnectionNodes == null ? new Vector3[0] : femaleConnectionNodes;
            this.Centeroid = this.Vertices.GetCentroid();
        }

        public void Initialize(IServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
