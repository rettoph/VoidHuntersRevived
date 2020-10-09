using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.LayerGroups;
using Guppy.UI.Entities;
using Guppy.UI.Layers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Services;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Client.Library.Scenes
{
    public class ClientGameScene : GameScene
    {
        #region Private Fields
        private ShipPartRenderService _shipPartRenderService;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _shipPartRenderService);

            // Create a ScreenLayer to hold the stage..
            this.Layers.Create<ScreenLayer>((l, p, c) =>
            {
                l.Group = new SingleLayerGroup(1);
                l.DrawOrder = 20;
            });

            this.Entities.Create<Stage>((s, p, d) =>
            {
                s.LayerGroup = 1;
            });
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _shipPartRenderService.Update(gameTime);
        }
        #endregion
    }
}
