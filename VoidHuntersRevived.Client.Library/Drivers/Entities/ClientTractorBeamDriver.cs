using FarseerPhysics.Dynamics;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.ConnectionNodes;
using Guppy;
using Guppy.Attributes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Extensions.Farseer;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities
{
    [IsDriver(typeof(TractorBeam))]
    public sealed class ClientTractorBeamDriver : Driver<TractorBeam>
    {
        #region Private Fields
        private ServerRender _server;
        private Body _body;
        #endregion

        #region Constructor
        public ClientTractorBeamDriver(ServerRender server, TractorBeam driven) : base(driven)
        {
            _server = server;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.driven.Events.TryAdd<ShipPart>("selected", this.HandleSelected);
            this.driven.Events.TryAdd<ShipPart>("selected:position:changed", this.HandleSelectedPositionUpdated);
        }
        #endregion

        #region Event Handlers
        private void HandleSelected(object sender, ShipPart arg)
        {
            _body = _server.GetBodyById(arg.BodyId);
        }

        private void HandleSelectedPositionUpdated(object sender, ShipPart arg)
        {
            _body.SetTransformIgnoreContacts(arg.Position, arg.Rotation);
        }
        #endregion
    }
}
