using FarseerPhysics;
using GalacticFighters.Client.Library.Scenes;
using GalacticFighters.Library.Configurations;
using GalacticFighters.Library.Entities.ShipParts;
using Guppy;
using Guppy.Attributes;
using Guppy.Loaders;
using Guppy.Utilities.Cameras;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalacticFighters.Client.Library.Drivers.Entities.ShipParts
{
    [IsDriver(typeof(ShipPart))]
    internal sealed class ClientShipPartDriver : Driver<ShipPart>
    {
        private ContentLoader _content;
        private Texture2D _texture;
        private Camera2D _camera;
        private SpriteBatch _spriteBatch;
        private Vector3 _position;
        private Vector2 _origin;
        private Boolean _textureLoaded;

        public ClientShipPartDriver(SpriteBatch spriteBatch, ClientGalacticFightersWorldScene scene, ContentLoader content, ShipPart driven) : base(driven)
        {
            _spriteBatch = spriteBatch;
            _camera = scene.Camera;
            _content = content;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.driven.Actions.TryAdd("health", this.HandleHealthAction);

            var config = (driven.Configuration.Data as ShipPartConfiguration);
            _texture = _content.TryGet<Texture2D>(config.Texture);

            if(_texture == default(Texture2D))
            {
                _textureLoaded = false;
            }
            else
            {
                _textureLoaded = true;
                _origin = Vector2.Zero;
                var verticies = config.Vertices.SelectMany(v => v);
                Vector2 min = ConvertUnits.ToDisplayUnits(new Vector2(verticies.Min(v => v.X), verticies.Min(v => v.Y)));
                Vector2 max = ConvertUnits.ToDisplayUnits(new Vector2(verticies.Max(v => v.X), verticies.Max(v => v.Y)));
                Vector2 center = (min + max) / 2;

                _origin = (new Vector2(_texture.Width, _texture.Height)/2) - center;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // Update the internal Vector3 data
            this.driven.Position.Deconstruct(out _position.X, out _position.Y);

            if (_textureLoaded && _camera.Frustum.Contains(_position).HasFlag(ContainmentType.Contains))
            {
                _spriteBatch.Draw(
                    texture: _texture,
                    position: this.driven.Position,
                    sourceRectangle: _texture.Bounds,
                    color: Color.Lerp(Color.Red, Color.Blue, this.driven.Health / 100),
                    rotation: this.driven.Rotation,
                    origin: _origin,
                    scale: 0.01f,
                    effects: SpriteEffects.None,
                    layerDepth: 0);
            }
        }

        private void HandleHealthAction(object sender, NetIncomingMessage arg)
        {
            this.driven.Health = arg.ReadSingle();
        }
    }
}
