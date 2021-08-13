using Guppy.DependencyInjection;
using Guppy.Enums;
using Guppy.Interfaces;
using Guppy.Network.Components;
using Guppy.Network.Peers;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.WorldObjects;

namespace VoidHuntersRevived.Client.Library.Components.Entities.Players
{
    internal sealed class UserPlayerLocalComponent : RemoteHostComponent<UserPlayer>
    {
        #region Private Fields
        private Camera2D _camera;
        private ClientPeer _client;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _camera);
            provider.Service(out _client);

            this.Entity.OnStatus[ServiceStatus.Initializing] += this.HandleEntityInitializing;
        }

        protected override void Release()
        {
            base.Release();

            this.Entity.OnStatus[ServiceStatus.Initializing] += this.HandleEntityInitializing;

            _client = default;
            _camera = default;
        }

        private void HandleEntityInitializing(IService sender, ServiceStatus old, ServiceStatus value)
        {
            if(_client.CurrentUser == this.Entity.User)
            {
                this.Entity.OnStatus[ServiceStatus.Releasing] += this.HandleEntityReleasing;
            }
        }

        private void HandleEntityReleasing(IService sender, ServiceStatus old, ServiceStatus value)
        {
            this.Entity.OnStatus[ServiceStatus.Releasing] -= this.HandleEntityReleasing;
        }
        #endregion
    }
}
