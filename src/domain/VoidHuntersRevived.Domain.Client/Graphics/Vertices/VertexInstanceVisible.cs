using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace VoidHuntersRevived.Domain.Client.Graphics.Vertices
{
    [StructLayout(LayoutKind.Explicit)]
    public struct VertexInstanceVisible : IVertexType
    {
        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        private static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(00, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3),
            new VertexElement(64, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(80, VertexElementFormat.Color, VertexElementUsage.Color, 1)
        );

        [FieldOffset(0)]
        public Matrix InstanceTranformation;

        [FieldOffset(64)]
        public uint PrimaryColor;

        [FieldOffset(80)]
        public uint SecondaryColor;
    }
}
