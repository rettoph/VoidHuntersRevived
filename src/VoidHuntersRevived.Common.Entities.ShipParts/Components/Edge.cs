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

        public Edge(Degree outDegree, Degree inDegree)
        {
            this.OutDegree = outDegree;
            this.InDegree = inDegree;
        }
    }
}
