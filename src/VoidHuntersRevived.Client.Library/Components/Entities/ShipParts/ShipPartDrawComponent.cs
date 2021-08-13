using Guppy;
using Guppy.DependencyInjection;
using Guppy.Enums;
using Guppy.Interfaces;
using Guppy.Utilities.Primitives;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Client.Library.Services;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Client.Library.Components.Entities.ShipParts
{
    internal sealed class ShipPartDrawComponent : Component<ShipPart>
    {
        #region Private Fields
        private ShipPartRenderService _renderer;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _renderer);

            this.Entity.OnDrawAt += this.DrawAt;
        }

        protected override void PostRelease()
        {
            base.PostRelease();

            this.Entity.OnDrawAt -= this.DrawAt;

            _renderer = default;
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
