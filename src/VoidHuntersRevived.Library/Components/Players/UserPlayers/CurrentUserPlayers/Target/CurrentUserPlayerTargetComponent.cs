using Guppy.CommandLine.Services;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.IO.Services;
using Guppy.Threading.Interfaces;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Components.Ships;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Messages.Commands;

namespace VoidHuntersRevived.Library.Components.Players.UserPlayers
{
    internal class CurrentUserPlayerTargetComponent : CurrentUserPlayerBaseComponent
    {
        #region Private Fields
        private MouseService _mouse;
        private Camera2D _camera;
        #endregion

        protected override void InitializeCurrentUser(ServiceProvider provider)
        {
            provider.Service(out _mouse);
            provider.Service(out _camera);

            this.Entity.OnShipChanged += this.HandleShipChanged;
        }

        protected override void UninitializeCurrentUser()
        {
            this.Entity.OnShipChanged -= this.HandleShipChanged;
        }

        #region Frame Methods
        private void Update(GameTime gameTime)
        {
            this.Entity.Ship.Components.Get<TargetComponent>().SetValue(_camera.Project(_mouse.Position));
        }
        #endregion

        #region Event Handlers
        private void HandleShipChanged(Player sender, Ship old, Ship value)
        {
            if(value is null)
            {
                this.Entity.OnUpdate -= this.Update;
            }

            if(value is not null)
            {
                this.Entity.OnUpdate += this.Update;
            }
        }
        #endregion
    }
}
