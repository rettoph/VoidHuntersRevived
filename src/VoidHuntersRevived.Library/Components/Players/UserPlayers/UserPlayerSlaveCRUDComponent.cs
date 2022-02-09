using Guppy.EntityComponent;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Network.Attributes;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Guppy.Network.Messages;
using Guppy.Network.Security;
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
    [NetworkAuthorizationRequired(NetworkAuthorization.Slave)]
    internal sealed class UserPlayerSlaveCRUDComponent : Component<UserPlayer>,
        IDataProcessor<UserPlayerCreatePacket>
    {
        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.Entity.Messages.RegisterProcessor<UserPlayerCreatePacket>(this);
        }

        protected override void PostUninitialize()
        {
            base.PostUninitialize();

            this.Entity.Messages.DeregisterProcessor<UserPlayerCreatePacket>(this);
        }
        #endregion

        #region Data Processors
        Boolean IDataProcessor<UserPlayerCreatePacket>.Process(UserPlayerCreatePacket message)
        {
            if(this.Entity.Pipe.Room.Users.TryGetById(message.UserId, out User user))
            {
                this.Entity.User = user;
                return true;
            }

            return false;
        }
        #endregion
    }
}
