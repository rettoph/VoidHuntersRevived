using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalacticFighters.Library.Configurations
{
    public class ShipPartConfiguration
    {
        public readonly List<Vertices> Vertices;
        public readonly ConnectionNodeConfiguration MaleConnectionNode;
        public readonly ConnectionNodeConfiguration[] FemaleConnectionNodes;

        public ShipPartConfiguration(
            Vertices vertices,
            ConnectionNodeConfiguration maleConnectionNode,
            ConnectionNodeConfiguration[] femaleConnectionNodes = null)
        {
            this.Vertices = new List<Vertices>();
            this.Vertices.Add(vertices);
            this.MaleConnectionNode = maleConnectionNode;
            this.FemaleConnectionNodes = femaleConnectionNodes == null ? new ConnectionNodeConfiguration[0] : femaleConnectionNodes;
        }

        public ShipPartConfiguration(
            List<Vertices> vertices,
            ConnectionNodeConfiguration maleConnectionNode,
            ConnectionNodeConfiguration[] femaleConnectionNodes = null)
        {
            this.Vertices = vertices;
            this.MaleConnectionNode = maleConnectionNode;
            this.FemaleConnectionNodes = femaleConnectionNodes == null ? new ConnectionNodeConfiguration[0] : femaleConnectionNodes;
        }
    }
}
