using Guppy.Collections;
using Guppy.Implementations;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using System.Linq;

namespace VoidHuntersRevived.Client.Library.Drivers
{
    public class ClientShipPartDriver : Driver
    {
        #region Private Fields
        private ShipPart _shipPart;
        private EntityCollection _entities;
        #endregion

        #region Constructors
        public ClientShipPartDriver(ShipPart parent, EntityCollection entities, IServiceProvider provider) : base(parent, provider)
        {
            _shipPart = parent;
            _entities = entities;
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            _shipPart.AddActionHandler("male-connection-node:detach", this.HandleMaleConnectionNodeDetachAction);
            _shipPart.AddActionHandler("male-connection-node:attach", this.HandleMaleConnectionNodeAttachAction);
        }
        #endregion

        #region Action Handlers
        private void HandleMaleConnectionNodeDetachAction(NetIncomingMessage obj)
        {
            // Only detatch if the ship part is connected to anything
            if (_shipPart.MaleConnectionNode.Target != null)
                _shipPart.DetatchFrom();
        }

        private void HandleMaleConnectionNodeAttachAction(NetIncomingMessage obj)
        {
            // Load the target female node...
            var target = (_entities.GetById(obj.ReadGuid()) as ShipPart)
                .FemaleConnectionNodes
                .First(f => f.Id == obj.PeekInt32());

            // If the male node is not already connected to that target, attempt to connect to it
            if (_shipPart.MaleConnectionNode.Target != target)
                _shipPart.AttatchTo(target);
        }
        #endregion
    }
}
