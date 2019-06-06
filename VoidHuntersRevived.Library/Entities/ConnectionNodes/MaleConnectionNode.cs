using System;
using System.Collections.Generic;
using System.Text;
using Guppy;
using Guppy.Configurations;
using Guppy.Extensions.DependencyInjection;
using Guppy.Loaders;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Entities.ConnectionNodes
{
    public class MaleConnectionNode : ConnectionNode
    {
        private Texture2D _texture;
        private IServiceProvider _provider;
        private SpriteBatch _spriteBatch;

        public MaleConnectionNode(ShipPart parent, float rotation, Vector2 position, EntityConfiguration configuration, Scene scene, IServiceProvider provider, ILogger logger, SpriteBatch spriteBatch = null) : base(parent, rotation, position, configuration, scene, provider, logger)
        {
            _provider = provider;
            _spriteBatch = spriteBatch;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _texture = _provider.GetLoader<ContentLoader>().Get<Texture2D>("texture:connection-node:male");
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spriteBatch.Draw(
                texture: _texture,
                position: this.WorldPosition,
                sourceRectangle: _texture.Bounds,
                color: Color.White,
                rotation: this.WorldRotation,
                origin: _texture.Bounds.Center.ToVector2(),
                scale: 0.01f,
                effects: SpriteEffects.None,
                layerDepth: 0);
        }
    }
}
