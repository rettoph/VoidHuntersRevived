using Guppy.DependencyInjection;
using Guppy.UI.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Scenes;
using VoidHuntersRevived.Library;

namespace VoidHuntersRevived.Client.Library
{
    public sealed class ClientVoidHuntersRevivedGame : VoidHuntersRevivedGame
    {
        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.Scenes.Create<MainMenuScene>();
        }
        #endregion
    }
}
