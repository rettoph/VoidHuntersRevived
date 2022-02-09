using Guppy.CommandLine.Services;
using Guppy.EntityComponent.DependencyInjection;
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
    internal class CurrentUserPlayerTractorBeamComponent : CurrentUserPlayerBaseComponent,
        IDataProcessor<TractorBeamRequestCommand>
    {
        #region Private Fields
        private CommandService _commands;
        #endregion

        protected override void InitializeCurrentUser(ServiceProvider provider)
        {
            provider.Service(out _commands);

            _commands.RegisterProcessor<TractorBeamRequestCommand>(this);
        }

        protected override void UninitializeCurrentUser()
        {
            _commands.DeregisterProcessor<TractorBeamRequestCommand>(this);
        }

        #region Command Processors
        bool IDataProcessor<TractorBeamRequestCommand>.Process(TractorBeamRequestCommand data)
        {
            switch (data.Type)
            {
                case Enums.TractorBeamStateType.Select:
                    this.Entity.Ship?.Components.Get<TractorBeamComponent>().EnqueueSelect();
                    break;
                case Enums.TractorBeamStateType.Deselect:
                    this.Entity.Ship?.Components.Get<TractorBeamComponent>().EnqueueDeselect();
                    break;
                default:
                    throw new NotImplementedException();
            }
            
            return true;
        }
        #endregion
    }
}
