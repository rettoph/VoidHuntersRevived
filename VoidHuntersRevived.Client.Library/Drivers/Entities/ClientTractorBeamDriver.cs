using FarseerPhysics.Dynamics;
using GalacticFighters.Client.Library.Utilities;
using GalacticFighters.Library.Entities;
using GalacticFighters.Library.Entities.ShipParts;
using Guppy;
using Guppy.Attributes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Client.Library.Drivers.Entities
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

        #region Frame Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.driven.Events.TryAdd<ShipPart>("selected", this.HandleSelected);
            this.driven.Events.TryAdd<Vector2>("selected:position:changed", this.HandleSelectedPositionUpdated);
        }
        #endregion

        #region Event Handlers
        private void HandleSelected(object sender, ShipPart arg)
        {
            _body = _server.GetBodyById(arg.BodyId);
        }

        private void HandleSelectedPositionUpdated(object sender, Vector2 arg)
        {
            _body.SetTransform(arg, _body.Rotation);
        }
        #endregion
    }
}
