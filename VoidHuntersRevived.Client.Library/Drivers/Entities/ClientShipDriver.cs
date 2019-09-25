using GalacticFighters.Library.Entities;
using GalacticFighters.Library.Entities.ShipParts;
using Guppy;
using Guppy.Attributes;
using Guppy.Collections;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Client.Library.Drivers.Entities
{
    [IsDriver(typeof(Ship))]
    public class ClientShipDriver : Driver<Ship>
    {
        #region Private Fields
        private EntityCollection _entities;
        #endregion

        #region Constructor
        public ClientShipDriver(EntityCollection entities, Ship driven) : base(driven)
        {
            _entities = entities;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.driven.Actions.TryAdd("bridge:changed", this.HandleBridgeChanged);
            this.driven.Actions.TryAdd("direction:changed", this.HandleDirectionChanged);
            this.driven.Actions.TryAdd("tractor-beam:selected", this.HandleTractorBeamSelected);
            this.driven.Actions.TryAdd("tractor-beam:released", this.HandleTractorBeamReleased);
            this.driven.Actions.TryAdd("tractor-beam:attached", this.HandleTractorBeamAttached);
        }
        #endregion

        #region Action Handlers
        private void HandleBridgeChanged(object sender, NetIncomingMessage im)
        {
            this.driven.ReadBridge(im);
        }

        private void HandleDirectionChanged(object sender, NetIncomingMessage im)
        {
            this.driven.ReadDirection(im);
        }

        private void HandleTractorBeamSelected(object sender, NetIncomingMessage arg)
        {
            this.driven.TractorBeam.SetOffset(arg.ReadVector2());
            this.driven.TractorBeam.TrySelect(arg.ReadEntity<ShipPart>(_entities));
        }

        private void HandleTractorBeamReleased(object sender, NetIncomingMessage arg)
        {
            this.driven.TractorBeam.SetOffset(arg.ReadVector2());
            this.driven.TractorBeam.TryRelease();
        }

        private void HandleTractorBeamAttached(object sender, NetIncomingMessage arg)
        {
            this.driven.TractorBeam.TryAttach(
                arg.ReadEntity<ShipPart>(_entities).FemaleConnectionNodes[arg.ReadInt32()]);
        }
        #endregion
    }
}
