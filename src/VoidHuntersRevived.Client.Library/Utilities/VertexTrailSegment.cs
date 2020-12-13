using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace VoidHuntersRevived.Client.Library.Utilities
{
    [StructLayout(LayoutKind.Explicit)]
    struct VertexTrailSegment : IVertexType
    {
        #region Private Fields
        [FieldOffset(0)]
        public Vector4 Position;

        [FieldOffset(16)]
        public Vector4 Color;

        [FieldOffset(0)]
        private Vector2 _worldPosition;

        [FieldOffset(32)]
        public Single RayLength;

        [FieldOffset(36)]
        public Vector2 Port;
        #endregion

        #region Public Properties
        #endregion

        #region IVertexType Implementation
        VertexDeclaration IVertexType.VertexDeclaration => VertexTrailSegment.VertexDeclaration;
        private static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Position, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.Color, 0),
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(32, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(36, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 2)
        );
        #endregion
    }
}
