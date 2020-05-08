using Guppy;
using Guppy.DependencyInjection;
using Guppy.UI.Components;
using Guppy.UI.Entities;
using Guppy.UI.Enums;
using Guppy.UI.Layers;
using Guppy.UI.Utilities.Units;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Pages;

namespace VoidHuntersRevived.Client.Library.Scenes
{
    public sealed class MainMenuScene : Scene
    {
        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            var ui = this.Layers.Create<StageLayer>();

            var stage = this.Entities.Create<Stage>();
            var paginator = stage.Children.Create<Paginator>();

            var title = paginator.Children.Create<TitlePage>();
            paginator.SetPage(title);
        }
        #endregion
    }
}
