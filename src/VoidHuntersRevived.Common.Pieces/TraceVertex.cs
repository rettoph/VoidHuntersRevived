using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Pieces
{
    public struct TraceVertex
    {
        public Vector2 Position;
        public bool Outer;

        public TraceVertex(Vector2 position, bool outer)
        {
            this.Position = position;
            this.Outer = outer;
        }
    }
}
