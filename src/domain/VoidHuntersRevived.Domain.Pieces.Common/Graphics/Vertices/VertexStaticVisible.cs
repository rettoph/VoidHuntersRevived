using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace VoidHuntersRevived.Domain.Pieces.Common.Graphics.Vertices
{
    [StructLayout(LayoutKind.Explicit)]
    public struct VertexStaticVisibleFlags
    {
        [FieldOffset(0)]
        public bool IsTrace;

        [FieldOffset(1)]
        public bool Undefined1;

        [FieldOffset(2)]
        public bool Undefined2;

        [FieldOffset(3)]
        public bool Undefined3;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct VertexStaticVisible : IVertexType
    {
        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        private static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Byte4, VertexElementUsage.BlendIndices, 0),
            new VertexElement(4, VertexElementFormat.Vector2, VertexElementUsage.Position, 0)
        );

        [FieldOffset(0)]
        public VertexStaticVisibleFlags Flags;

        [FieldOffset(4)]
        public Vector2 Position;

        public VertexStaticVisible(Vector2 position)
        {
            this.Position = position;
        }

        public VertexStaticVisible(Vector2 position, bool isTrace)
        {
            this.Position = position;
            this.Flags.IsTrace = isTrace;
        }
    }
}
