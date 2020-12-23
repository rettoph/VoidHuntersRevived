using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Client.Library.Scenes
{
    public class MainMenuScene : GraphicsGameScene
    {
        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.settings.Set<NetworkAuthorization>(NetworkAuthorization.Master);
            this.settings.Set<HostType>(HostType.Local);
        }

        protected override void Release()
        {
            base.Release();

            this.settings.Set<NetworkAuthorization>(NetworkAuthorization.Slave);
            this.settings.Set<HostType>(HostType.Remote);
        }
        #endregion
    }
}
