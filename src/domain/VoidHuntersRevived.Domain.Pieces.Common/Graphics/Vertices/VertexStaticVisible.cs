using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

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

        private static readonly VertexDeclaration VertexDeclaration = new(
            new VertexElement(0, VertexElementFormat.Byte4, VertexElementUsage.BlendIndices, 0),
            new VertexElement(4, VertexElementFormat.Vector2, VertexElementUsage.Position, 0)
        );

        [FieldOffset(0)]
        public VertexStaticVisibleFlags Flags;

        [FieldOffset(4)]
        public Vector2 Position;

        // TODO: Investigate updating JsonSerializerOptions.IncludeFields so this attribute is not needed
        // There might be cascading issues by changing that value?
        [JsonInclude]
        [FieldOffset(4)]
        public float X;

        [JsonInclude]
        [FieldOffset(8)]
        public float Y;

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
