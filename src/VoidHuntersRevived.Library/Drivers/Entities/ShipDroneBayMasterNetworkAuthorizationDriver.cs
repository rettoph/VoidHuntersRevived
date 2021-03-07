using Guppy;
using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    /// <summary>
    /// Handle the fighter bay action request that can be invoked by a ship.
    /// </summary>
    internal sealed class ShipDroneBayMasterNetworkAuthorizationDriver : MasterNetworkAuthorizationDriver<Ship>
    {
        #region Lifecycle Methods
        protected override void InitializeRemote(Ship driven, ServiceProvider provider)
        {
            base.InitializeRemote(driven, provider);

            this.driven.Actions.Add(VHR.Actions.Ship.OnLaunchFighters, this.HandleOnLaunchFightersAction);
        }
        protected override void ReleaseRemote(Ship driven)
        {
            base.ReleaseRemote(driven);

            this.driven.Actions.Remove(VHR.Actions.Ship.OnLaunchFighters, this.HandleOnLaunchFightersAction);
        }
        #endregion

        #region Event Handlers
        private void HandleOnLaunchFightersAction(Ship sender, object args)
        {
            // Broadcast a message through the network.

        }
        #endregion
    }
}
