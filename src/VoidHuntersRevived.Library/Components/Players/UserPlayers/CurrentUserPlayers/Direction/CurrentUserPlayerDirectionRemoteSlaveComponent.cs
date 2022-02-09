using Guppy.CommandLine.Services;
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
using VoidHuntersRevived.Library.Messages.Commands;
using VoidHuntersRevived.Library.Messages.Network;
using VoidHuntersRevived.Library.Messages.Requests;

namespace VoidHuntersRevived.Library.Components.Players.UserPlayers
{
    [HostTypeRequired(HostType.Remote)]
    [NetworkAuthorizationRequired(NetworkAuthorization.Slave)]
    internal sealed class CurrentUserPlayerDirectionRemoteSlaveComponent : CurrentUserPlayerShipBaseComponent<DirectionComponent>
    {
        protected override void AddNewShipComponent(DirectionComponent directionComponent)
        {
            base.AddNewShipComponent(directionComponent);

            directionComponent.OnDirectionChanged += this.HandleDirectionChanged;
        }

        protected override void RemoveOldShipComponent(DirectionComponent directionComponent)
        {
            base.RemoveOldShipComponent(directionComponent);

            directionComponent.OnDirectionChanged -= this.HandleDirectionChanged;
        }

        private void HandleDirectionChanged(DirectionComponent sender, DirectionRequest data)
        {
            if(data.RequestHost == HostType.Remote)
            { // The direction came form the server, no need to send it back...
                return;
            }

            this.Entity.SendMessage<UserPlayerDirectionRequestMessage>(new UserPlayerDirectionRequestMessage()
            {
                Direction = data.Direction,
                State = data.State
            });
        }
    }
}
