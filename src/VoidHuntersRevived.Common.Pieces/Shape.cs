using Microsoft.Xna.Framework;
using Svelto.DataStructures;

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
