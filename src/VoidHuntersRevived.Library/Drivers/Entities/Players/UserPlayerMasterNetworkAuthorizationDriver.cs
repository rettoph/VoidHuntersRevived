using Guppy.DependencyInjection;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;

namespace VoidHuntersRevived.Library.Drivers.Entities.Players
{
    internal sealed class UserPlayerMasterNetworkAuthorizationDriver : MasterNetworkAuthorizationDriver<UserPlayer>
    {
        #region Lifecycle Methods
        protected override void Initialize(UserPlayer driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            this.driven.Actions.Set("update:ship:target:request", this.HandleUpdateShiTargetRequestMessage);
        }
        #endregion

        #region Message Handlers
        private void HandleUpdateShiTargetRequestMessage(NetIncomingMessage obj)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
