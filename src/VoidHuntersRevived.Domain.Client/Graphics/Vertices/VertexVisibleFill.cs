using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Domain.Client.Graphics.Vertices
{
    [StructLayout(LayoutKind.Explicit)]
    public struct VertexVisibleFill : IVertexType
    {
        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        private static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Color, 0),
            new VertexElement(16, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );

        /// <summary>
        /// The color of the current trail.
        /// </summary>
        [FieldOffset(0)]
        private Vector4 _color;

        public Color Color
        {
            set => _color = value.ToVector4();
        }

        /// <summary>
        /// The world position of the current trail particle.
        /// </summary>
        [FieldOffset(16)]
        public Vector2 Position;
    }
}
