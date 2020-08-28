using Guppy;
using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    public abstract class NetworkEntityAuthorizationDriver <TNetworkEntity> : BaseAuthorizationDriver<TNetworkEntity>
        where TNetworkEntity : NetworkEntity
    {
        #region Lifecycle Methods
        protected override void Configure(object driven, ServiceProvider provider)
        {
            base.Configure(driven, provider);

            this.driven.OnPreAuthorizationChanged += this.HandlePreAuthorizationChanged;
            this.driven.OnAuthorizationChanged += this.HandleAuthorizationChanged;
        }
        #endregion

        #region Helper Methods
        protected override GameAuthorization GetDefaultAuthorization()
            => this.driven.Authorization;
        #endregion

        #region Event Handlers
        private void HandleAuthorizationChanged(NetworkEntity sender, GameAuthorization old, GameAuthorization value)
            => this.ConfigureAuthorization(value);

        private void HandlePreAuthorizationChanged(NetworkEntity sender, GameAuthorization arg)
            => this.DisposeAuthorization();
        #endregion
    }
}
