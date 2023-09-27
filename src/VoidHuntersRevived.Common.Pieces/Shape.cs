using Microsoft.Xna.Framework;
using Svelto.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Components;

namespace VoidHuntersRevived.Common.Pieces
{
    public struct Shape : IDisposable
    {
        public required NativeDynamicArrayCast<Vector2> Vertices { get; init; }

        public void Dispose()
        {
            this.Vertices.Dispose();
        }
    }
}
