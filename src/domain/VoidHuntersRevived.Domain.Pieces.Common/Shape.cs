using Microsoft.Xna.Framework;
using Svelto.Common;
using Svelto.DataStructures;
using VoidHuntersRevived.Common.Extensions.Svelto;

namespace VoidHuntersRevived.Domain.Pieces.Common
{
    public struct Shape : IDisposable
    {
        public required NativeDynamicArrayCast<Vector2> Vertices { get; init; }

        public void Dispose()
        {
            this.Vertices.Dispose();
        }

        public Shape Clone()
        {
            return new Shape()
            {
                Vertices = this.Vertices.Clone(Allocator.Persistent)
            };
        }
    }
}
