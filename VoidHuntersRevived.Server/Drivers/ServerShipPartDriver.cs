using Guppy.Implementations;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Utilities.ConnectionNodes;
using Guppy.Network.Extensions.Lidgren;

namespace VoidHuntersRevived.Server.Drivers
{
    /// <summary>
    /// Simple driver used to send messages to the client when
    /// a ship-part is attached or detached.
    /// </summary>
    public class ServerShipPartDriver : Driver
    {
        #region Private Fields
        private ShipPart _shipPart;
        #endregion

        #region Constructors
        public ServerShipPartDriver(ShipPart parent, IServiceProvider provider) : base(parent, provider)
        {
            _shipPart = parent;
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            _shipPart.MaleConnectionNode.OnAttatchedTo += this.HandleMaleConnectionNodeAttachedTo;
            _shipPart.MaleConnectionNode.OnDetachedFrom += this.HandleMaleConnectionNodeDetachedFrom;
        }
        #endregion

        #region Event Handlers
        private void HandleMaleConnectionNodeDetachedFrom(object sender, EventArgs e)
        {
            var action = _shipPart.CreateActionMessage("male-connection-node:detach");
        }

        private void HandleMaleConnectionNodeAttachedTo(object sender, FemaleConnectionNode e)
        {
            var action = _shipPart.CreateActionMessage("male-connection-node:attach");
            action.Write(e.Parent.Id);
            action.Write(e.Id);
        }
        #endregion

        public override void Dispose()
        {
            base.Dispose();

            _shipPart.MaleConnectionNode.OnAttatchedTo -= this.HandleMaleConnectionNodeAttachedTo;
            _shipPart.MaleConnectionNode.OnDetachedFrom -= this.HandleMaleConnectionNodeDetachedFrom;
        }
    }
}
