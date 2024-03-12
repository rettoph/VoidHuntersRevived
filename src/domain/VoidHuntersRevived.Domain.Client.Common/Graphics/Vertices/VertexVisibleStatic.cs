using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace VoidHuntersRevived.Domain.Client.Common.Graphics.Vertices
{
    [StructLayout(LayoutKind.Explicit)]
    public struct VertexVisibleStatic : IVertexType
    {
        private static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0)
        );

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        [FieldOffset(0)]
        public Vector3 Position;

        public VertexVisibleStatic(Vector2 position, float z)
        {
            this.Position = new Vector3(position, z);
        }
    }
}
