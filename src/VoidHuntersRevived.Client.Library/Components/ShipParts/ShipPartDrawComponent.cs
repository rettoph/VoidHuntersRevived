using Guppy;
using Guppy.EntityComponent;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Interfaces;
using Guppy.Utilities.Primitives;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Client.Library.Services;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Client.Library.Components.ShipParts
{
    internal sealed class ShipPartDrawComponent : Component<ShipPart>
    {
        #region Private Fields
        private ShipPartRenderService _renderer;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _renderer);

            this.Entity.OnDrawAt += this.DrawAt;
        }

        protected override void PostUninitialize()
        {
            base.PostUninitialize();

            this.Entity.OnDrawAt -= this.DrawAt;
        }
        #endregion

        #region Frame Methods
        private void DrawAt(GameTime gameTime, ref Matrix worldTransformation)
        {
            _renderer.Render(this.Entity, ref worldTransformation);
        }
        #endregion
    }
}
