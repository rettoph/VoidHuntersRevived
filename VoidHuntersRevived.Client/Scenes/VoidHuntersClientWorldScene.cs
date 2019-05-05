using Guppy.UI.Elements;
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
            var c = stage.Content.CreateElement<Container>(100, 100, 100, 100);
            stage.Content.CreateElement<TextInput>(250, 250, 300, 30);
            c.StateBlacklist = Guppy.UI.Enums.ElementState.Active;

            layer.Debug = true;
        }
    }
}
