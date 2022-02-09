using Guppy.CommandLine.Services;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Guppy.Network.Utilities;
using Guppy.Threading.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Components.Ships;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Messages.Commands;
using VoidHuntersRevived.Library.Messages.Network;

namespace VoidHuntersRevived.Library.Components.Players.UserPlayers
{
    [HostTypeRequired(HostType.Remote)]
    [NetworkAuthorizationRequired(NetworkAuthorization.Slave)]
    internal sealed class CurrentUserPlayerTargetRemoteSlaveComponent : CurrentUserPlayerBaseComponent,
        IDataFactory<UserPlayerTargetRequestMessage>
    {
        private NetworkEntityMessagePinger _pinger;

        protected override void InitializeCurrentUser(ServiceProvider provider)
        {
            this.Entity.OnShipChanged += this.HandleShipChanged;

            _pinger = this.Entity.Messages.RegisterPinger<UserPlayerTargetRequestMessage>(
                this,
                Globals.Constants.Intervals.MinimumShipTargetPingInterval,
                Globals.Constants.Intervals.MaximumShipTargetPingInterval,
                false);
        }

        protected override void UninitializeCurrentUser()
        {
            this.Entity.OnShipChanged -= this.HandleShipChanged;
        }

        #region Command Processors
        UserPlayerTargetRequestMessage IDataFactory<UserPlayerTargetRequestMessage>.Create()
        {
            return new UserPlayerTargetRequestMessage()
            {
                Target = this.Entity.Ship.Components.Get<TargetComponent>().Value
            };
        }
        #endregion

        #region Events
        private void HandleShipChanged(Player sender, Ship old, Ship value)
        {
            if(value is null)
            {
                _pinger.Enabled = false;
            }

            if(value is not null)
            {
                _pinger.Enabled = true;
            }
        }
        #endregion
    }
}
