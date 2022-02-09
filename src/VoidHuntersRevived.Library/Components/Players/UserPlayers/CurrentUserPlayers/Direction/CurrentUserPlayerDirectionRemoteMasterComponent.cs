using Guppy.CommandLine.Services;
using Guppy.EntityComponent;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Guppy.Threading.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Components.Ships;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Messages.Commands;
using VoidHuntersRevived.Library.Messages.Network;

namespace VoidHuntersRevived.Library.Components.Players.UserPlayers
{
    [HostTypeRequired(HostType.Remote)]
    [NetworkAuthorizationRequired(NetworkAuthorization.Master)]
    internal sealed class CurrentUserPlayerDirectionRemoteMasterComponent : Component<UserPlayer>,
        IDataProcessor<UserPlayerDirectionRequestMessage>
    {
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.Entity.Messages.RegisterProcessor<UserPlayerDirectionRequestMessage>(this);
        }

        protected override void Uninitialize()
        {
            base.Uninitialize();

            this.Entity.Messages.RegisterProcessor<UserPlayerDirectionRequestMessage>(this);
        }

        #region Command Processors
        bool IDataProcessor<UserPlayerDirectionRequestMessage>.Process(UserPlayerDirectionRequestMessage data)
        {
            this.Entity.Ship?.Components.Get<DirectionComponent>().EnqueueRequest(data.Direction, data.State, HostType.Remote);

            return true;
        }
        #endregion
    }
}
