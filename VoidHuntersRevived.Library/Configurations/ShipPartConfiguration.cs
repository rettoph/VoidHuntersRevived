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
        public readonly List<Vertices> Vertices;
        public readonly ConnectionNodeConfiguration MaleConnectionNode;
        public readonly ConnectionNodeConfiguration[] FemaleConnectionNodes;
        public readonly Vector2 Centeroid;
        public readonly Single Density;

        public ShipPartConfiguration(
            Vertices vertices,
            ConnectionNodeConfiguration maleConnectionNode,
            ConnectionNodeConfiguration[] femaleConnectionNodes = null,
            Single density = 0.5f)
        {
            this.Vertices = new List<Vertices>();
            this.Vertices.Add(vertices);
            this.MaleConnectionNode = maleConnectionNode;
            this.FemaleConnectionNodes = femaleConnectionNodes == null ? new ConnectionNodeConfiguration[0] : femaleConnectionNodes;
            this.Centeroid = new Vertices(vertices).GetCentroid();
            this.Density = density;
        }

        public ShipPartConfiguration(
            List<Vertices> vertices,
            ConnectionNodeConfiguration maleConnectionNode,
            ConnectionNodeConfiguration[] femaleConnectionNodes = null,
            Single density = 0.5f)
        {
            this.Vertices = vertices;
            this.MaleConnectionNode = maleConnectionNode;
            this.FemaleConnectionNodes = femaleConnectionNodes == null ? new ConnectionNodeConfiguration[0] : femaleConnectionNodes;
            this.Centeroid = new Vertices(vertices.SelectMany(i => i)).GetCentroid();
            this.Density = density;
        }

        public void Initialize(IServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
