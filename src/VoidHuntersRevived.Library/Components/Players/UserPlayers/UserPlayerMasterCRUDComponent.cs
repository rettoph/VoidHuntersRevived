using Guppy.EntityComponent;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Network.Attributes;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Guppy.Network.Messages;
using Guppy.Threading.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Messages.Network.Packets;

namespace VoidHuntersRevived.Library.Components.Players.UserPlayers
{
    [HostTypeRequired(HostType.Remote)]
    [NetworkAuthorizationRequired(NetworkAuthorization.Master)]
    internal sealed class UserPlayerMasterCRUDComponent : Component<UserPlayer>,
        IDataFactory<UserPlayerCreatePacket>
    {
        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.Entity.Messages.RegisterPacket<UserPlayerCreatePacket, CreateNetworkEntityMessage>(this);
        }

        protected override void PostUninitialize()
        {
            base.PostUninitialize();

            this.Entity.Messages.DeregisterPacket<UserPlayerCreatePacket, CreateNetworkEntityMessage>(this);
        }
        #endregion

        #region Network Methods
        UserPlayerCreatePacket IDataFactory<UserPlayerCreatePacket>.Create()
        {
            return new UserPlayerCreatePacket()
            {
                UserId = this.Entity.User.Id
            };
        }
        #endregion
    }
}
