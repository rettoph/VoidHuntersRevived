using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Components
{
    public class Edge
    {
        public readonly Degree OutDegree;
        public readonly Degree InDegree;

        public Matrix LocalTransformation => CalculateLocalTransformation(InDegree, OutDegree);

        public Edge(Degree outDegree, Degree inDegree)
        {
            this.OutDegree = outDegree;
            this.InDegree = inDegree;
        }

        public static Matrix CalculateLocalTransformation(Degree inDegree, Degree outDegree)
        {
            Matrix transformation = Matrix.Invert(inDegree.Configuration.Transformation);
            transformation *= Matrix.CreateRotationZ(MathHelper.Pi);
            transformation *= outDegree.LocalTransformation;

            return transformation;
        }
    }
}
