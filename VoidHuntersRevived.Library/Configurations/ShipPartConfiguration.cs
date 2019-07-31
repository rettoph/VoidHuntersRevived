using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Guppy.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoidHuntersRevived.Library.Configurations
{
    public class ShipPartConfiguration : IEntityData
    {
        public readonly List<List<Vector2>> Vertices;
        public readonly Vector3 MaleConnectionNode;
        public readonly Vector3[] FemaleConnectionNodes;
        public readonly Vector2 Centeroid;
        public readonly Single Density;

        public ShipPartConfiguration(
            List<Vector2> vertices,
            Vector3 maleConnectionNode,
            Vector3[] femaleConnectionNodes = null,
            Single density = 0.5f)
        {
            this.Vertices = new List<List<Vector2>>();
            this.Vertices.Add(vertices);
            this.MaleConnectionNode = maleConnectionNode;
            this.FemaleConnectionNodes = femaleConnectionNodes == null ? new Vector3[0] : femaleConnectionNodes;
            this.Centeroid = new Vertices(vertices).GetCentroid();
            this.Density = density;
        }

        public ShipPartConfiguration(
            List<List<Vector2>> vertices,
            Vector3 maleConnectionNode,
            Vector3[] femaleConnectionNodes = null,
            Single density = 0.5f)
        {
            this.Vertices = vertices;
            this.MaleConnectionNode = maleConnectionNode;
            this.FemaleConnectionNodes = femaleConnectionNodes == null ? new Vector3[0] : femaleConnectionNodes;
            this.Centeroid = new Vertices(vertices.SelectMany(i => i)).GetCentroid();
            this.Density = density;
        }

        public void Initialize(IServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
