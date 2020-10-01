using Guppy.DependencyInjection;
using Guppy.LayerGroups;
using Guppy.UI.Entities;
using Guppy.UI.Layers;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Client.Library.Scenes
{
    public class ClientGameScene : GameScene
    {
        #region Frame Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

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
    }
}
