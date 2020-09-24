using Guppy;
using Guppy.Lists;
using Guppy.DependencyInjection;
using Guppy.Network;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Scenes;
using Guppy.Extensions.DependencyInjection;

namespace VoidHuntersRevived.Library.Drivers.Entities.Players
{
    internal sealed class UserPlayerMinimumAuthorizationNetworkDriver : BaseAuthorizationDriver<UserPlayer>
    {
        #region Private Fields
        private GameScene _scene;
        #endregion

        #region Lifecycle Methods
        protected override void ConfigureMinimum(ServiceProvider provider)
        {
            base.ConfigureMinimum(provider);

            provider.Service(out _scene);

            this.driven.Actions.Set("update:user", this.ReadUser);
        }

        protected override void DisposeMinimum()
        {
            base.DisposeMinimum();

            this.driven.Actions.Remove("update:user");
        }
        #endregion

        #region Network Methods
        private void ReadUser(NetIncomingMessage obj)
        {
            if(obj.ReadExists())
                this.driven.User = _scene.Group.Users.GetById(obj.ReadGuid());
            else
                this.driven.User = null;
        }
        #endregion
    }
}
