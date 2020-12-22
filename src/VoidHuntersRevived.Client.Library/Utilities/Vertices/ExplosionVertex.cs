using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace VoidHuntersRevived.Client.Library.Utilities.Vertices
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ExplosionVertex : IVertexType
    {
        #region Fields
        [FieldOffset(0)]
        private Vector4 _color;

        [FieldOffset(16)]
        public Vector2 Position;

        [FieldOffset(24)]
        public Single Direction;

        [FieldOffset(28)]
        public Single Magnitude;

        [FieldOffset(32)]
        public Single CreatedTimestamp;

        [FieldOffset(36)]
        public Single Alpha;

        [FieldOffset(40)]
        public Single MaxAge;
        #endregion

        #region Public Properties
        public Color Color
        {
            set => _color = value.ToVector4();
        }
        #endregion

        #region IVertexType Implementation
        VertexDeclaration IVertexType.VertexDeclaration => ExplosionVertex.VertexDeclaration;
        private static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Color, 0),
            new VertexElement(16, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(24, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(28, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 2),
            new VertexElement(32, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 3),
            new VertexElement(36, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 4),
            new VertexElement(40, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 5)
        );
        #endregion
    }
}