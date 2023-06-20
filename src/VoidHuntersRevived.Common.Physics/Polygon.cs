using Svelto.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Physics
{
    public class Polygon
    {
        public Vertices Vertices;
        public Fix64 Density;

        public Polygon(Vertices vertices, Fix64 density)
        {
            this.Vertices = vertices;
            this.Density = density;
        }
    }
}
