using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace VoidHuntersRevived.Game.Client.Common.Graphics.Vertices
{
    [StructLayout(LayoutKind.Explicit)]
    public struct VertexStaticVisible : IVertexType
    {
        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        private const uint IsTraceFlag = 0x00000001;
        private const uint IsOuterFlag = 0x00000002;
        private static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Byte4, VertexElementUsage.BlendIndices, 0)
        );

        [FieldOffset(0)]
        public Vector3 Position;

        [FieldOffset(12)]
        private uint _flags;

        [FieldOffset(12)]
        public bool Trace;

        public VertexStaticVisible(Vector2 position, float zIndex)
        {
            this.Position = new Vector3(position, zIndex);
            _flags = 0;
        }

        public VertexStaticVisible(Vector2 position, float zIndex, bool trace)
        {
            this.Position = new Vector3(position, zIndex);
            this.Trace = trace;
        }
    }
}
