using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Collision.Shapes;

namespace VoidHuntersRevived.Domain.Entities.Components
{
    public partial class Hull
    {
        public class Shape
        {
            private static readonly PolygonShape Default = new PolygonShape(0);

            public PolygonShape Polygon { get; set; } = Default;
        }
    }
}
