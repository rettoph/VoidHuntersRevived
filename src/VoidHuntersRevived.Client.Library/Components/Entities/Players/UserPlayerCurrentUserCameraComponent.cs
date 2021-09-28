using Guppy.CommandLine.Services;
using Guppy.DependencyInjection;
using Guppy.IO.Services;
using Guppy.Network.Components;
using Guppy.Network.Peers;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using VoidHuntersRevived.Library.Components.Entities.Ships;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Enums;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Entities.Aether;
using tainicom.Aether.Physics2D.Collision;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Structs;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Extensions.Lidgren;
using VoidHuntersRevived.Library.Components.Entities.Players;

namespace VoidHuntersRevived.Client.Library.Components.Entities.Players
{
    internal sealed class UserPlayerCurrentUserCameraComponent : UserPlayerCurrentUserBaseComponent
    {
        #region Private Fields
        private Camera2D _camera;
        private MouseService _mouse;
        #endregion

        #region Lifecycle Methods
        protected override void InitializeCurrentUser(GuppyServiceProvider provider)
        {
            base.InitializeCurrentUser(provider);

            provider.Service(out _camera);
            provider.Service(out _mouse);

            this.Entity.OnUpdate += this.Update;
        }

        protected override void ReleaseCurrentUser()
        {
            base.ReleaseCurrentUser();

            this.Entity.OnUpdate -= this.Update;

            _camera = default;
            _mouse = default;
        }
        #endregion

        #region Frame Methods
        private void Update(GameTime gameTime)
        {
            if (this.Entity.Ship == default)
                return;

            _camera.MoveTo(this.Entity.Ship.Chain.Position);
        }
        #endregion
    }
}
