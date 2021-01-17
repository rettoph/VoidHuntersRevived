using Guppy.Utilities;
using Guppy.Extensions.Utilities;
using Guppy.Extensions.Microsoft.Xna.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Utilities.Cameras;

namespace VoidHuntersRevived.Builder.Contexts
{
    /// <summary>
    /// Contains side specific data for a shape
    /// under construction.
    /// </summary>
    public struct SideContext
    {
        #region Public Fields
        /// <summary>
        /// The internal length of a specific side.
        /// </summary>
        public Single Length;

        /// <summary>
        /// The rotation of this specific side
        /// relative to the previous side.
        /// </summary>
        public Single Rotation;
        #endregion

        #region Frame Methods
        /// <summary>
        /// Render the current side details at the specified position.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="worldRotation"></param>
        /// <param name="primitiveBatch"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="font"></param>
        /// <param name="color"></param>
        /// <param name="camera"></param>
        public void Draw(
            Vector2 position,
            Single worldRotation,
            PrimitiveBatch<VertexPositionColor> primitiveBatch,
            SpriteBatch spriteBatch,
            SpriteFont font,
            Color color,
            Camera2D camera)
        {
            var start = position;
            var end = position + (Vector2.UnitX * this.Length).RotateTo(worldRotation + MathHelper.Pi - this.Rotation);

            spriteBatch.DrawString(
                spriteFont: font,
                text: MathHelper.ToDegrees(this.Rotation).ToString("##0.##°"),
                position: position,
                color: color,
                rotation: 0,
                origin: Vector2.Zero,
                scale: 1 / camera.Zoom,
                effects: SpriteEffects.None,
                layerDepth: 0);

            spriteBatch.DrawString(
                spriteFont: font,
                text: this.Length.ToString("##0.##"),
                position: (start + end) / 2,
                color: color,
                rotation: 0,
                origin: Vector2.Zero,
                scale: 1 / camera.Zoom,
                effects: SpriteEffects.None,
                layerDepth: 0);

            primitiveBatch.DrawLine(Color.Lerp(color, Color.Black, 0.5f), start, end);
        }
        #endregion
    }
}
