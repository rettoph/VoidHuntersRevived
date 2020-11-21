using Guppy;
using Guppy.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Services;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities.ShipParts
{
    /// <summary>
    /// Primary driver responsible for rendering
    /// ShipPart instances.
    /// </summary>
    internal sealed class ShipPartGraphicsDriver : Driver<ShipPart>
    {
        #region Private Fields
        private ShipPartRenderService _renderer;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ShipPart driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            provider.Service(out _renderer);

            _renderer.ValidateConfiguration(this.driven);

            this.driven.OnDraw += this.Draw;
        }

        protected override void Release(ShipPart driven)
        {
            base.Release(driven);

            this.driven.OnDraw -= this.Draw;
        }
        #endregion

        #region Frame Methods
        private void Draw(GameTime gameTime)
        {
            _renderer.Draw(this.driven);
        }
        #endregion
    }
}
