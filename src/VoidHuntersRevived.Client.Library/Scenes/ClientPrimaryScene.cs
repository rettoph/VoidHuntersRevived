using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Extensions.Utilities;
using Guppy.IO.Services;
using Guppy.Utilities;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Chunks;
using VoidHuntersRevived.Library.Contexts.ShipParts;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Hulls;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Library.Services;
using tainicom.Aether.Physics2D.Diagnostics;
using VoidHuntersRevived.Client.Library.Entities.Aether;

namespace VoidHuntersRevived.Client.Library.Scenes
{
    public class ClientPrimaryScene : PrimaryScene
    {
        #region Private Fields
        private PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private SpriteBatch _spriteBatch;
        private Camera2D _camera;
        private MouseService _mouse;
        private AetherDebugView _debugView;
        BasicEffect _effect;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _primitiveBatch);
            provider.Service(out _spriteBatch);
            provider.Service(out _camera);
            provider.Service(out _mouse);
            provider.Service(out _debugView);

            _effect = new BasicEffect(provider.GetService<GraphicsDevice>())
            {
                TextureEnabled = true,
                VertexColorEnabled = true
            };

            _camera.MinZoom = 0.25f;
            _camera.MaxZoom = 400f;
            _camera.ZoomTo(5f);
            _camera.MoveTo(Chunk.Size / 2, Chunk.Size / 2);

            _mouse.OnScrollWheelValueChanged += (m, d) =>
            {
                _camera.ZoomBy((Single)Math.Pow(1.5, d.Delta / 120));
            };
        }
        #endregion

        #region Frame Methods
        protected override void PreDraw(GameTime gameTime)
        {
            base.PreDraw(gameTime);

            _camera.TryClean(gameTime);
            _primitiveBatch.Begin(_camera, BlendState.AlphaBlend);

            _effect.Projection = _camera.Projection;
            _effect.World = _camera.World;
            _effect.View = _camera.View;

            _spriteBatch.Begin(effect: _effect);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _debugView.TryDraw(gameTime);
        }

        protected override void PostDraw(GameTime gameTime)
        {
            base.PostDraw(gameTime);

            _primitiveBatch.End();
            _spriteBatch.End();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        #endregion
    }
}
