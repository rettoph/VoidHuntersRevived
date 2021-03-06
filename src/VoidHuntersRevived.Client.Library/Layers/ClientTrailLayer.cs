using Guppy.DependencyInjection;
using Guppy.Utilities;
using Guppy.Utilities.Cameras;
using Guppy.Extensions.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Graphics.Effects;
using VoidHuntersRevived.Client.Library.Graphics.Vertices;
using VoidHuntersRevived.Client.Library.Services;
using VoidHuntersRevived.Library.Layers;

namespace VoidHuntersRevived.Client.Library.Layers
{
    class ClientTrailLayer : TrailLayer
    {
        #region Private Fields
        private TrailService _trails;
        private PrimitiveBatch<VertexTrail, TrailEffect> _primitiveBatch;
        private Camera2D _camera;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _trails);
            provider.Service(out _primitiveBatch);
            provider.Service(out _camera);
        }
        #endregion

        #region Frame Methods
        protected override void PreDraw(GameTime gameTime)
        {
            base.PreDraw(gameTime);

            _primitiveBatch.Effect.CurrentTimestamp = (Single)gameTime.TotalGameTime.TotalSeconds;

            _primitiveBatch.Begin(_camera, BlendState.NonPremultiplied);
        }

        protected override void PostDraw(GameTime gameTime)
        {
            base.PostDraw(gameTime);

            _primitiveBatch.End();
        }

        protected override void PreUpdate(GameTime gameTime)
        {
            base.PreUpdate(gameTime);

            _trails.TryUpdate(gameTime);
        }
        #endregion
    }
}
