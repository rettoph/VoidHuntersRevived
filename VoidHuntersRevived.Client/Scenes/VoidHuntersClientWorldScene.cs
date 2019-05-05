using Guppy.UI.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Layers;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Client.Scenes
{
    public class VoidHuntersClientWorldScene : VoidHuntersWorldScene
    {
        public VoidHuntersClientWorldScene(IServiceProvider provider) : base(provider)
        {
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            var layer = this.layers.Create<CameraLayer>();
            var stage = this.entities.Create("ui:stage") as Stage;

            layer.Debug = true;
        }
    }
}
