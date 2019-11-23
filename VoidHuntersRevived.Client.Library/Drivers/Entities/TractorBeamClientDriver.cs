using Guppy;
using Guppy.Attributes;
using Guppy.Collections;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities
{
    /// <summary>
    /// Manage action messages recieved from the server
    /// related to the trator beam
    /// </summary>
    [IsDriver(typeof(TractorBeam))]
    internal sealed class TractorBeamClientDriver : Driver<TractorBeam>
    {
        #region Private Fields
        private EntityCollection _entities;
        #endregion

        #region Constructor
        public TractorBeamClientDriver(EntityCollection entities, TractorBeam driven) : base(driven)
        {
            _entities = entities;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.driven.Ship.Actions.TryAdd("tractor-beam:selected", this.HandleSelectedAction);
            this.driven.Ship.Actions.TryAdd("tractor-beam:released", this.HandleReleasedAction);
        }
        #endregion

        #region Action Handlers
        private void HandleSelectedAction(object sender, NetIncomingMessage im)
        {
            this.driven.Ship.SetTarget(im.ReadVector2());
            this.driven.Ship.TractorBeam.TrySelect(im.ReadEntity<ShipPart>(_entities));
        }
        private void HandleReleasedAction(object sender, NetIncomingMessage im)
        {
            this.driven.Ship.SetTarget(im.ReadVector2());
            this.driven.Ship.TractorBeam.TryRelease();
        }
        #endregion
    }
}
