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

        [FieldOffset(0)]
        private Vector2 _worldPosition;

        [FieldOffset(32)]
        private Vector2 _port;

        [FieldOffset(40)]
        private Vector2 _starboard;
        #endregion

        #region Public Properties
        #endregion

        #region Constructor
        public VertexTrailSegment(Vector2 position, Color color, Vector2 port, Vector2 starboard, TrailSegment segment)
        {
            _worldPosition = position;
            _position = new Vector4(position, 0, 1);
            _color = color.ToVector4();
            _port = port;
            _starboard = starboard;
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
            new VertexElement(40, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 2)
        );
        #endregion
    }
}
