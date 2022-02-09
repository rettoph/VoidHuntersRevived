using Guppy.CommandLine.Services;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Network.Enums;
using Guppy.Threading.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Components.Ships;
using VoidHuntersRevived.Library.Messages.Commands;

namespace VoidHuntersRevived.Library.Components.Players.UserPlayers
{
    internal sealed class CurrentUserPlayerDirectionComponent : CurrentUserPlayerBaseComponent,
        IDataProcessor<DirectionRequestCommand>
    {
        protected override void InitializeCurrentUser(ServiceProvider provider)
        {
            this.commands.RegisterProcessor<DirectionRequestCommand>(this);
        }

        protected override void UninitializeCurrentUser()
        {
            this.commands.DeregisterProcessor<DirectionRequestCommand>(this);
        }

        #region Command Processors
        bool IDataProcessor<DirectionRequestCommand>.Process(DirectionRequestCommand data)
        {
            this.Entity.Ship?.Components.Get<DirectionComponent>().EnqueueRequest(data.Direction, data.State, HostType.Local);
            return true;
        }
        #endregion
    }
}
