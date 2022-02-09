using Guppy;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.IO.Services;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Components.Players.UserPlayers;

namespace VoidHuntersRevived.Client.Library.Components.Players
{
    internal sealed class UserPlayerCurrentUserCameraComponent : CurrentUserPlayerBaseComponent
    {
        #region Private Fields
        private Camera2D _camera;
        private MouseService _mouse;
        private Scene _scene;
        #endregion

        #region Lifecycle Methods
        protected override void InitializeCurrentUser(ServiceProvider provider)
        {
            provider.Service(out _camera);
            provider.Service(out _mouse);
            provider.Service(out _scene);

            _scene.OnPreDraw += this.Update;
        }

        protected override void UninitializeCurrentUser()
        {
            _camera = default;
            _mouse = default;

            _scene.OnPreDraw -= this.Update;
        }
        #endregion

        #region Frame Methods
        private void Update(GameTime gameTime)
        {
            if (this.Entity.Ship is null)
                return;

            _camera.MoveTo(this.Entity.Ship.Chain.Body.LocalInstance.WorldCenter);
            _camera.TryClean(gameTime);
        }
        #endregion
    }
}
