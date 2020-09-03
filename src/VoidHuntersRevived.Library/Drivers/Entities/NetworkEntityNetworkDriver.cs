using Guppy;
using Guppy.DependencyInjection;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    public abstract class NetworkEntityNetworkDriver<TNetworkEntity> : BaseAuthorizationDriver<TNetworkEntity>
        where TNetworkEntity : NetworkEntity
    {
        #region Private Fields
        private List<GameAuthorizationActions> _actions;
        #endregion

        #region Lifecycle Methods
        protected override void Configure(object driven, ServiceProvider provider)
        {
            base.Configure(driven, provider);

            _actions = new List<GameAuthorizationActions>();

            this.driven.OnAuthorizationChanged += this.HandleAuthorizationChanged;
        }

        protected override void Dispose()
        {
            base.Dispose();

            _actions.ForEach(a =>
            {
                this.driven.Actions.Remove(a.Type);
                a.Dispose();
            });

            this.driven.OnAuthorizationChanged -= this.HandleAuthorizationChanged;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Add an internal action with varying responses based on the entities
        /// internal network authorization.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="map"></param>
        protected void AddAction(String type, Boolean required = false, Int32 sizeInBits = 0, params(GameAuthorization authorization, Action<NetIncomingMessage> handler)[] map)
        {
            // Create new GameAuthorizaion instance based on recieved authorization handler map
            var action = new GameAuthorizationActions(
                type: type, 
                actions: map.ToDictionary(keySelector: kvp => kvp.authorization, elementSelector: kvp => kvp.handler), 
                required: required,
                sizeInBits: sizeInBits);

            this.driven.Actions.Set(action.Type, action.DoAction);
            action.ConfigureAuthorization(this.GetGameAuthorization());

            // Internally store new game action..
            _actions.Add(action);
        }

        protected override GameAuthorization GetGameAuthorization()
            => this.driven.Authorization;
        #endregion

        #region Event Handlers
        private void HandleAuthorizationChanged(NetworkEntity sender, GameAuthorization old, GameAuthorization value)
        {
            _actions.ForEach(a => a.ConfigureAuthorization(value));

            this.UpdateAuthorization(value);
        }
        #endregion
    }
}
