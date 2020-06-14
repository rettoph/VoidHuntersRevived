using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.Utilities;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities
{
    /// <summary>
    /// Render base world objects...
    /// </summary>
    internal sealed class WorldEntityGraphicsDriver : Driver<WorldEntity>
    {
        #region Private Fields
        private PrimitiveBatch _primitiveBatch;
        #endregion

        #region Lifecyle Methods
        protected override void Configure(object driven, ServiceProvider provider)
        {
            base.Configure(driven, provider);

            provider.Service(out _primitiveBatch);

            this.driven.OnDraw += this.Draw;
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.driven.OnDraw -= this.Draw;
        }
        #endregion

        #region Frame Methods
        private void Draw(GameTime gameTime)
        {
            _primitiveBatch.DrawRectangle(new Rectangle(Point.Zero, this.driven.Size.ToPoint()), Color.Gray);
        }
        #endregion
    }
}
