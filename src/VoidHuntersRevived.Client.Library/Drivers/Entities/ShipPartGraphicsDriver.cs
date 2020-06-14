using FarseerPhysics;
using FarseerPhysics.Common;
using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.Utilities;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities
{
    /// <summary>
    /// Primary driver responsible for rendering
    /// ShipPart instances.
    /// </summary>
    internal sealed class ShipPartGraphicsDriver : Driver<ShipPart>
    {
        #region Private Fields
        private ShipPartRenderer _renderer;
        #endregion

        #region Lifecycle Methods
        protected override void Configure(object driven, ServiceProvider provider)
        {
            base.Configure(driven, provider);

            provider.Service(out _renderer);

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
            => _renderer.Draw(this.driven);
        #endregion
    }
}
