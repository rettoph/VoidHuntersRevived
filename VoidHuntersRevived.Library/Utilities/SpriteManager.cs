using FarseerPhysics;
using FarseerPhysics.Common;
using Guppy.Loaders;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoidHuntersRevived.Library.Utilities
{
    /// <summary>
    /// Simple class used to render sprites within the farseer
    /// </summary>
    public sealed class SpriteManager
    {
        private SpriteBatch _spriteBatch;
        private Texture2D _texture;
        private ContentLoader _content;
        private Boolean _textureLoaded;
        private Vector2 _origin;
        private Vector3 _position;

        public SpriteManager(SpriteBatch spriteBatch, ContentLoader content)
        {
            _spriteBatch = spriteBatch;
            _content = content;
        }

        #region Load Methods
        public void Load(String texture, IReadOnlyCollection<Vertices> vertices)
        {
            this.Load(texture, vertices.SelectMany(v => v));
        }
        public void Load(String texture, IEnumerable<Vector2> vertices)
        {
            _texture = _content.TryGet<Texture2D>(texture);

            if (_texture == default(Texture2D))
            {
                _textureLoaded = false;
            }
            else
            {
                _textureLoaded = true;
                _origin = Vector2.Zero;

                Vector2 min = ConvertUnits.ToDisplayUnits(new Vector2(vertices.Min(v => v.X), vertices.Min(v => v.Y)));
                Vector2 max = ConvertUnits.ToDisplayUnits(new Vector2(vertices.Max(v => v.X), vertices.Max(v => v.Y)));
                Vector2 center = (min + max) / 2;

                _origin = (new Vector2(_texture.Width, _texture.Height) / 2) - center;
            }
        }
        #endregion

        #region Draw Methods
        public void Draw(Vector2 position, Single rotation, Color color, Camera camera)
        {
            position.Deconstruct(out _position.X, out _position.Y);

            if (camera.Frustum.Contains(_position).HasFlag(ContainmentType.Contains))
                this.Draw(position, rotation, color);
        }

        public void Draw(Vector2 position, Single rotation, Color color)
        {
            if (_textureLoaded)
                _spriteBatch.Draw(
                    texture: _texture,
                    position: position,
                    sourceRectangle: _texture.Bounds,
                    color: color,
                    rotation: rotation,
                    origin: _origin,
                    scale: 0.01f,
                    effects: SpriteEffects.None,
                    layerDepth: 0);
        }
        #endregion
    }
}
