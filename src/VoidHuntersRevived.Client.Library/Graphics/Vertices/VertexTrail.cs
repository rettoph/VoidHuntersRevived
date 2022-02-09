using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Client.Library.Graphics.Vertices
{
    /// <summary>
    /// The primary vertex used to mape trail particles.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct VertexTrail : IVertexType
    {
        #region Private Fields
        /// <summary>
        /// The color of the current trail.
        /// </summary>
        [FieldOffset(0)]
        private Vector4 _color;
        #endregion

        #region Public Fields
        /// <summary>
        /// The world position of the current trail particle.
        /// </summary>
        [FieldOffset(16)]
        public Vector2 Position;

        /// <summary>
        /// The velocity of the current trail particle.
        /// </summary>
        [FieldOffset(24)]
        public Vector2 Velocity;

        /// <summary>
        /// The speed at which the current particle should spread
        /// </summary>
        [FieldOffset(32)]
        public Single SpreadSpeed;

        /// <summary>
        /// The direction at which the current particle should spread
        /// </summary>
        [FieldOffset(36)]
        public Single SpreadDirection;

        /// <summary>
        /// The timestamp at which this particle was created.
        /// </summary>
        [FieldOffset(40)]
        public Single CreatedTimestamp;

        /// <summary>
        /// The maximum age of the current particle
        /// </summary>
        [FieldOffset(44)]
        public Single MaxAge;
        #endregion

        #region Public Properties
        /// <summary>
        /// The color of the current trail.
        /// </summary>
        public Color Color
        {
            set => _color = value.ToVector4();
        }
        #endregion



        #region IVertexType Implementation
        VertexDeclaration IVertexType.VertexDeclaration => VertexTrail.VertexDeclaration;

        private static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Color, 0),
            new VertexElement(16, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(32, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 2),
            new VertexElement(36, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 3),
            new VertexElement(40, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 4),
            new VertexElement(44, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 5)
        );
        #endregion
    }
}
