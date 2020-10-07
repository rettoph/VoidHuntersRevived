using Guppy;
using Guppy.DependencyInjection;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Controllers;
using Guppy.Extensions.DependencyInjection;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities.Controllers
{
    internal sealed class TractorBeamGraphicsDriver : Driver<TractorBeam>
    {
        #region Private Fields
        private PrimitiveBatch _primitiveBatch;
        #endregion

        #region Lifecycle Methods
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
            _primitiveBatch.DrawLine(Color.Red, this.driven.Position - Vector2.UnitX, this.driven.Position + Vector2.UnitX);
            _primitiveBatch.DrawLine(Color.Red, this.driven.Position - Vector2.UnitY, this.driven.Position + Vector2.UnitY);
        }
        #endregion
    }
}
