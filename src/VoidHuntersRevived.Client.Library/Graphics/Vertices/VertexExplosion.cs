using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using VoidHuntersRevived.Library.Contexts;

namespace VoidHuntersRevived.Windows.Library.Graphics.Vertices
{
    /// <summary>
    /// The primary vertex used to mape explosion particles.
    /// Loosely based off <see cref="ExplosionContext"/>
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct VertexExplosion : IVertexType
    {
        #region Private Fields
        /// <summary>
        /// The color of the current explosion.
        /// </summary>
        [FieldOffset(0)]
        private Vector4 _color;
        #endregion

        #region Public Fields
        /// <summary>
        /// The world position of the current explosion.
        /// </summary>
        [FieldOffset(16)]
        public Vector2 Position;

        /// <summary>
        /// The velocity at which the explosion should travel
        /// </summary>
        [FieldOffset(24)]
        public Vector2 Velocity;

        /// <summary>
        /// The direction this particular particle is expanding outward.
        /// </summary>
        [FieldOffset(32)]
        public Single Direction;

        /// <summary>
        /// The maximum size of the explosion.
        /// </summary>
        [FieldOffset(36)]
        public Single MaxRadius;

        /// <summary>
        /// The timestamp at which the particle was created.
        /// Used to calculate its current age.
        /// </summary>
        [FieldOffset(40)]
        public Single CreatedTimestamp;

        /// <summary>
        /// The maximum age allowed (in seconds) before the explosion fully dissipates.
        /// </summary>
        [FieldOffset(44)]
        public Single MaxAge;
        #endregion

        #region Public Properties
        /// <summary>
        /// The color of the current explosion.
        /// </summary>
        public Color Color
        {
            set => _color = value.ToVector4();
        }
        #endregion

        #region IVertexType Implementation
        VertexDeclaration IVertexType.VertexDeclaration => VertexExplosion.VertexDeclaration;

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
