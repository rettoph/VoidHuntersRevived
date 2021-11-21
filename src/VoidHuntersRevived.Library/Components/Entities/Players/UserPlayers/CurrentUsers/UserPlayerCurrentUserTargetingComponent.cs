using Guppy.DependencyInjection;
using Guppy.IO.Services;
using Guppy.Network.Enums;
using Guppy.Network.Extensions.DependencyInjection;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Utilities;
using Guppy.Utilities.Cameras;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Components.Entities.Ships;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Globals.Constants;

namespace VoidHuntersRevived.Library.Components.Entities.Players
{
    internal sealed class UserPlayerCurrentUserTargetingComponent : UserPlayerCurrentUserBaseComponent
    {
        #region Private Fields
        private Camera2D _camera;
        private MouseService _mouse;
        private Broadcast _broadcast;
        private Boolean _dirty;
        private ShipTargetingComponent _targeting;
        #endregion

        #region Lifecycle Methods
        protected override void InitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.InitializeRemote(provider, networkAuthorization);

            this.Entity.Messages.Add(
                messageType: Messages.UserPlayer.RequestTargetChangedAction,
                defaultContext: Guppy.Network.Constants.MessageContexts.InternalUnreliableDefault);

            if (networkAuthorization == NetworkAuthorization.Master)
            {
                this.Entity.Messages[Messages.UserPlayer.RequestTargetChangedAction].OnRead += this.ReadCurrentUserRequestTargetChangedActionMessage;

                this.Entity.OnShipChanged += this.HandleShipChanged;
            }
                

            if(networkAuthorization == NetworkAuthorization.Slave)
            {
                this.Entity.Messages[Messages.UserPlayer.RequestTargetChangedAction].OnWrite += this.WriteCurrentUserRequestTargetChangedActionMessage;
            }
        }

        protected override void ReleaseRemote(NetworkAuthorization networkAuthorization)
        {
            base.ReleaseRemote(networkAuthorization);

            if (networkAuthorization == NetworkAuthorization.Master)
            {
                this.HandleShipChanged(default, this.Entity.Ship, default);

                this.Entity.OnShipChanged -= this.HandleShipChanged;
            }
        }

        protected override void InitializeCurrentUser(GuppyServiceProvider provider)
        {
            base.InitializeCurrentUser(provider);

            provider.Service(out _camera);
            provider.Service(out _mouse);

            _dirty = false;

            this.Entity.OnUpdate += this.Update;
            this.Entity.OnShipChanged += this.HandleCurrentUserShipChanged;
        }

        protected override void ReleaseCurrentUser()
        {
            base.ReleaseCurrentUser();

            this.Entity.OnUpdate -= this.Update;

            _camera = default;
            _mouse = default;
        }

        protected override void InitializeRemoteCurrentUser(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.InitializeRemoteCurrentUser(provider, networkAuthorization);

            if(networkAuthorization == NetworkAuthorization.Slave)
            {
                _broadcast = provider.GetBroadcast(Messages.UserPlayer.RequestTargetChangedAction);

                this.Entity.OnShipChanged += this.HandleCurrentUserShipChanged;

                this.HandleCurrentUserShipChanged(default, default, this.Entity.Ship);
            }
                
        }

        protected override void ReleaseRemoteCurrentUser(NetworkAuthorization networkAuthorization)
        {
            base.ReleaseRemoteCurrentUser(networkAuthorization);

            if (networkAuthorization == NetworkAuthorization.Slave)
            {
                this.HandleCurrentUserShipChanged(default, this.Entity.Ship, default);

                this.Entity.OnShipChanged -= this.HandleCurrentUserShipChanged;

                _broadcast = default;
            }
        }
        #endregion

        #region Frame Methods
        private void Update(GameTime gameTime)
        {
            // TODO: Optimize this
            if (this.Entity.Ship == default)
                return;

            this.Entity.Ship.Components.Get<ShipTargetingComponent>().Target = _camera.Unproject(_mouse.Position);
        }
        #endregion

        #region Network Methods
        private void ReadCurrentUserRequestTargetChangedActionMessage(MessageTypeManager sender, NetIncomingMessage im)
        {
            if(im.ReadExists())
            {
                Vector2 target = im.ReadVector2();
                
                if(_targeting != default)
                {
                    _targeting.Target = target;
                }
            }
        }

        private void WriteCurrentUserRequestTargetChangedActionMessage(MessageTypeManager sender, NetOutgoingMessage om)
        {
            if(om.WriteExists(_targeting))
            {
                om.Write(_targeting.Target);
            }
        }
        #endregion

        #region Helper Methods
        private void Clean(NetOutgoingMessage obj)
        {
            _dirty = false;
        }
        #endregion

        #region Events
        private void HandleCurrentUserShipChanged(Player sender, Ship old, Ship value)
        {
            // Unbind from the old ship...
            if(old != default)
            {
                _targeting.OnTargetChanged -= this.HandleTargetChanged;
            }

            this.HandleShipChanged(sender, old, value);

            // Bind to new ship...
            if (value != default)
            {
                _targeting.OnTargetChanged += this.HandleTargetChanged;
            }
        }

        private void HandleShipChanged(Player sender, Ship old, Ship value)
        {
            // Update internal references to n ew ship...
            _targeting = value?.Components.Get<ShipTargetingComponent>();
        }

        private void HandleTargetChanged(Ship sender, Vector2 args)
        {
            if (_dirty)
                return;

            _dirty = true;
            _broadcast.Enqueue(this.Entity, this.Clean);
        }
        #endregion
    }
}
