using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace VoidHuntersRevived.Game.Client.Common.Graphics.Vertices
{
    [StructLayout(LayoutKind.Explicit)]
    public struct VertexInstanceVisible : IVertexType
    {
        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
        private static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Single, VertexElementUsage.Color, 0),
            new VertexElement(4, VertexElementFormat.Single, VertexElementUsage.Color, 1),
            new VertexElement(8, VertexElementFormat.Single, VertexElementUsage.Color, 2),

            new VertexElement(12 + (16 * 0), VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(12 + (16 * 1), VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
            new VertexElement(12 + (16 * 2), VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
            new VertexElement(12 + (16 * 3), VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3)
        );

        [FieldOffset(0)]
        public uint PrimaryColor;

        [FieldOffset(4)]
        public uint SecondaryColor;

        [FieldOffset(8)]
        public float Z;

        [FieldOffset(12)]
        public Matrix LocalTransformation;
    }
}
