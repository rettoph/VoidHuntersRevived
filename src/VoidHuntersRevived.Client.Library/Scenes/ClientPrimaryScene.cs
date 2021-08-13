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

namespace VoidHuntersRevived.Client.Library.Scenes
{
    public class ClientPrimaryScene : PrimaryScene
    {
        #region Private Fields
        private PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private Camera2D _camera;
        Mouse _mouse;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _primitiveBatch);
            provider.Service(out _camera);
            provider.Service(out _mouse);

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
        }

        protected override void PostDraw(GameTime gameTime)
        {
            base.PostDraw(gameTime);

            _primitiveBatch.End();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        #endregion
    }
}
