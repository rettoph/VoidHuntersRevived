using Guppy.CommandLine.Services;
using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Network.Components;
using Guppy.Network.Contexts;
using Guppy.Network.Enums;
using Guppy.Network.Interfaces;
using Guppy.Network.Utilities;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;

namespace VoidHuntersRevived.Library.Components.Entities.Players
{
    public abstract class UserPlayerCurrentUserBaseComponent : NetworkComponent<UserPlayer>
    {
        #region Private Fields
        private Boolean _currentUser;
        #endregion

        #region Protected Fields
        protected Boolean currentUser => _currentUser;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(GuppyServiceProvider provider)
        {
            // Determin if the current Entity is the peer's CurrentUser.
            // This comes before the base.Initialize call simply because InitializeRemote happens first.
            _currentUser = this.hostType == HostType.Local || provider.GetService<IPeer>().CurrentUser == this.Entity.User;

            base.Initialize(provider);

            if (_currentUser)
                this.InitializeCurrentUser(provider);
        }

        protected override void Release()
        {
            base.Release();

            if (_currentUser)
                this.ReleaseCurrentUser();
        }

        protected override void InitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.InitializeRemote(provider, networkAuthorization);

            if(_currentUser)
                this.InitializeRemoteCurrentUser(provider, networkAuthorization);
        }

        protected override void ReleaseRemote(NetworkAuthorization networkAuthorization)
        {
            base.ReleaseRemote(networkAuthorization);

            if (_currentUser)
                this.ReleaseRemoteCurrentUser(networkAuthorization);
        }

        protected virtual void InitializeCurrentUser(GuppyServiceProvider provider)
        {
            //
        }

        protected virtual void ReleaseCurrentUser()
        {
            //
        }

        protected virtual void InitializeRemoteCurrentUser(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            //
        }

        protected virtual void ReleaseRemoteCurrentUser(NetworkAuthorization networkAuthorization)
        {
            //
        }
        #endregion
    }
}
