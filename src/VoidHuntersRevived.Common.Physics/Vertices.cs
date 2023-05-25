using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Physics
{
    public class Vertices : List<FixVector2>
    {
        public Vertices() : base()
        {
        }
        public Vertices(IEnumerable<FixVector2> collection) : base(collection)
        {
        }
    }
}
