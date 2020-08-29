using Guppy;
using Guppy.DependencyInjection;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    public abstract class NetworkEntityAuthorizationDriver<TNetworkEntity> : BaseAuthorizationDriver<TNetworkEntity>
        where TNetworkEntity : NetworkEntity
    {
        #region Private Fields
        private List<GameAuthorizationActions> _actions;
        #endregion

        #region Lifecycle Methods
        protected override void Configure(object driven, ServiceProvider provider)
        {
            base.Configure(driven, provider);

            this.driven.OnAuthorizationChanged += this.HandleAuthorizationChanged;
        }

        protected override void Dispose()
        {
            base.Dispose();

            _actions.ForEach(a => this.driven.Actions.Remove(a.Type));

            this.driven.OnAuthorizationChanged -= this.HandleAuthorizationChanged;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Add an internal action with varying responses based on the entities
        /// internal network authorization.
        /// </summary>
        /// <param name="map"></param>
        protected void AddAction(String type, Dictionary<GameAuthorization, Action<NetIncomingMessage>> map, Boolean required = false)
        {
            var action = new GameAuthorizationActions(type, this.driven.Authorization, map, required);
            this.driven.Actions.Set(action.Type, action.DoAction);
            _actions.Add(action);
        }

        protected override GameAuthorization GetDefaultAuthorization()
            => this.driven.Authorization;
        #endregion

        #region Event Handlers
        private void HandleAuthorizationChanged(NetworkEntity sender, GameAuthorization old, GameAuthorization value)
        {
            this.ConfigureAuthorization(value);
        }
        #endregion
    }
}
