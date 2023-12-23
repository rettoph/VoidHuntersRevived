using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace VoidHuntersRevived.Domain.Client.Graphics.Vertices
{
    [StructLayout(LayoutKind.Explicit)]
    public struct VertexVisible : IVertexType
    {
        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        private static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(8, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 1)
        );

        [FieldOffset(0)]
        public Vector2 Position;

        [FieldOffset(8)]
        private BitVector32 _flags;

        public bool Trace
        {
            set
            {
                _flags[1] = value;
            }
        }

        public bool Outer
        {
            set
            {
                _flags[1] = true;
                _flags[2] = value;
            }
        }
    }
}
