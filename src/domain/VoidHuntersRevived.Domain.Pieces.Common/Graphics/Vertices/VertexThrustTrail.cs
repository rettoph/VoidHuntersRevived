using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Pieces.Common.Graphics.Vertices
{
    [StructLayout(LayoutKind.Explicit)]
    public struct VertexThrustTrail : IVertexType, IEntityComponent
    {
        public VertexDeclaration VertexDeclaration => throw new NotImplementedException();
    }
}
