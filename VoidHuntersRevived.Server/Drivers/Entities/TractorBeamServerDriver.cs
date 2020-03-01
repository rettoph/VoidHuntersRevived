using Guppy;
using Guppy.Attributes;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Server.Drivers.Entities
{
    [IsDriver(typeof(TractorBeam))]
    internal sealed class TractorBeamServerDriver : Driver<TractorBeam>
    {
        #region Constructor
        public TractorBeamServerDriver(TractorBeam driven) : base(driven)
        {
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.driven.OnSelected += this.HandleSelected;
            this.driven.OnReleased += this.HandleReleased;
            this.driven.OnAttached += this.HandleAttached;
        }

        public override void Dispose()
        {
            base.Dispose();

            this.driven.OnSelected -= this.HandleSelected;
            this.driven.OnReleased -= this.HandleReleased;
            this.driven.OnAttached -= this.HandleAttached;
        }
        #endregion

        #region Event Handlers
        private void HandleSelected(object sender, ShipPart arg)
        {
            var action = this.driven.Ship.Actions.Create("tractor-beam:selected", NetDeliveryMethod.ReliableUnordered, 4);
            action.Write(this.driven.Ship.Target);
            action.Write(arg);
        }
        private void HandleReleased(object sender, ShipPart arg)
        {
            var action = this.driven.Ship.Actions.Create("tractor-beam:released", NetDeliveryMethod.ReliableUnordered, 4);
            action.Write(this.driven.Ship.Target);
        }

        private void HandleAttached(object sender, ConnectionNode arg)
        {
            var action = this.driven.Ship.Actions.Create("tractor-beam:attached", NetDeliveryMethod.ReliableUnordered, 4);
            action.Write(this.driven.Ship.Target);
            action.Write(arg.Parent);
            action.Write(arg.Id);
        }
        #endregion
    }
}
