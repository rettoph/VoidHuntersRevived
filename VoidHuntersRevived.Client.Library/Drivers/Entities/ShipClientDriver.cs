using Guppy;
using Guppy.Attributes;
using Guppy.Collections;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities
{
    /// <summary>
    /// Manage all recieved ship actions from the server
    /// </summary>
    [IsDriver(typeof(Ship))]
    internal sealed class ShipClientDriver : Driver<Ship>
    {
        #region Private Fields
        private EntityCollection _entities;
        #endregion

        #region Constructor
        public ShipClientDriver(EntityCollection entities, Ship driven) : base(driven)
        {
            _entities = entities;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.driven.Actions.TryAdd("target:changed", this.HandleTargetChangedAction);
            this.driven.Actions.TryAdd("firing:changed", this.HandleFiringChangedAction);
            this.driven.Actions.TryAdd("direction:changed", this.HandleDirectionChangedAction);
            this.driven.Actions.TryAdd("bridge:changed", this.HandleBridgeChangedAction);
        }
        #endregion

        #region Action Handlers 
        private void HandleTargetChangedAction(object sender, NetIncomingMessage im)
        {
            this.driven.SetTarget(im.ReadVector2());
        }

        private void HandleFiringChangedAction(object sender, NetIncomingMessage im)
        {
            this.driven.SetTarget(im.ReadVector2());
            this.driven.SetFiring(im.ReadBoolean());
        }

        private void HandleDirectionChangedAction(object sender, NetIncomingMessage arg)
        {
            this.driven.ReadDirection(arg);
        }

        private void HandleBridgeChangedAction(object sender, NetIncomingMessage arg)
        {
            this.driven.SetBridge(
                target: arg.ReadEntity<ShipPart>(_entities));
        }
        #endregion
    }
}
