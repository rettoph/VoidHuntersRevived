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
        private Vector4 _position;

        [FieldOffset(16)]
        private Vector4 _color;

        [FieldOffset(32)]
        private Vector2 _segmentStart;

        [FieldOffset(40)]
        private Single _slope;
        #endregion

        #region Public Properties
        public Color Color
        {
            set => _color = value.ToVector4();
        }

        public Vector2 Position
        {
            set => _position = new Vector4(value, 0, 1);
        }

        public Vector2 SegmentStart
        {
            set => _segmentStart = value;
        }

        public Single Slope
        {
            set => _slope = value;
        }
        #endregion

        #region IVertexType Implementation
        VertexDeclaration IVertexType.VertexDeclaration => VertexTrailSegment.VertexDeclaration;
        private static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Position, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.Color, 0),
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(32, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(40, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 2)
        );
        #endregion
    }
}
