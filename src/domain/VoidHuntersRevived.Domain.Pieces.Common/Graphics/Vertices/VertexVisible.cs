using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Common.FloatingPoint;

namespace VoidHuntersRevived.Domain.Pieces.Common.Graphics.Vertices
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct VertexVisible : IVertexType, IEntityComponent
    {
        readonly VertexDeclaration IVertexType.VertexDeclaration => _vertexDeclaration;
        private static readonly VertexDeclaration _vertexDeclaration = new(
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Position, 1),
            new VertexElement(16, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(20, VertexElementFormat.Color, VertexElementUsage.Color, 1)
        );

        [FieldOffset(0)]
        public Transform2D Transform2D;

        [FieldOffset(16)]
        public Color PrimaryColor;

        [FieldOffset(20)]
        public Color SecondaryColor;
    }
}