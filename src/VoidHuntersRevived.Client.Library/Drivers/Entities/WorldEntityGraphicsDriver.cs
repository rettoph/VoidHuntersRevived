using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.Utilities;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Services;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities
{
    /// <summary>
    /// Render base world objects...
    /// </summary>
    internal sealed class WorldEntityGraphicsDriver : Driver<WorldEntity>
    {
        #region Private Fields
        private ExplosionRenderService _explosions;
        private PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private Rectangle _bounds;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(WorldEntity driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            provider.Service(out _primitiveBatch);
            provider.Service(out _explosions);

            this.driven.OnDraw += this.Draw;
            this.driven.OnSizeChanged += this.HandleSizeChanged;
            this.driven.OnExplosionCreated += this.HandleExplosionCreated;

            this.CleanBounds();
        }

        protected override void Release(WorldEntity driven)
        {
            base.Release(driven);

            this.driven.OnDraw -= this.Draw;
            this.driven.OnSizeChanged -= this.HandleSizeChanged;
            this.driven.OnExplosionCreated -= this.HandleExplosionCreated;

            _primitiveBatch = null;
            _explosions = null;
        }
        #endregion

        #region Frame Methods
        private void Draw(GameTime gameTime)
            => _primitiveBatch.TraceRectangle(Color.Gray, _bounds);
        #endregion

        #region Helper Methods
        private void CleanBounds()
        {
            _bounds = new Rectangle(Point.Zero, this.driven.Size.ToPoint());
        }
        #endregion

        #region Event Handlers
        private void HandleSizeChanged(WorldEntity sender, Vector2 args)
            => this.CleanBounds();

        private void HandleExplosionCreated(WorldEntity sender, ref WorldEntity.ExplosionData data, IEnumerable<ShipPart> targets, GameTime gameTime)
            => _explosions.Configure(data, gameTime);
        #endregion
    }
}
