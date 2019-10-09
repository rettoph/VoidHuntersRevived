using GalacticFighters.Library.Entities;
using GalacticFighters.Library.Entities.ShipParts;
using Guppy;
using Guppy.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Network.Extensions.Lidgren;
using GalacticFighters.Library.Entities.ShipParts.ConnectionNodes;
using Microsoft.Xna.Framework;

namespace GalacticFighters.Server.Drivers.Entities
{
    [IsDriver(typeof(Ship))]
    public class ServerShipDriver : Driver<Ship>
    {
        #region private Fields
        private Vector2 _oldTarget;
        #endregion

        #region Constructor
        public ServerShipDriver(Ship driven) : base(driven)
        {
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.driven.Events.TryAdd<ShipPart>("bridge:changed", this.HandleBridgeChanged);
            this.driven.Events.TryAdd<Ship.Direction>("direction:changed", this.HandleDirectionChanged);
            this.driven.Events.TryAdd<Boolean>("firing:changed", this.HandleFiringChanged);
            this.driven.TractorBeam.Events.TryAdd<ShipPart>("selected", this.HandleTractorBeamSelected);
            this.driven.TractorBeam.Events.TryAdd<ShipPart>("released", this.HandleTractorBeamReleased);
            this.driven.TractorBeam.Events.TryAdd<FemaleConnectionNode>("attached", this.HandleTractorBeamAttached);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(_oldTarget != this.driven.TargetOffset)
            {
                var action = this.driven.Actions.Create("target:changed");
                action.Write(this.driven.TargetOffset);

                _oldTarget = this.driven.TargetOffset;
            }
        }
        #endregion

        #region Event Handlers
        private void HandleBridgeChanged(object sender, ShipPart bridge)
        {
            this.driven.WriteBridge(this.driven.Actions.Create("bridge:changed"));
        }

        private void HandleDirectionChanged(object sender, Ship.Direction direction)
        {
            this.driven.WriteDirection(this.driven.Actions.Create("direction:changed"), direction);
        }

        private void HandleTractorBeamSelected(object sender, ShipPart arg)
        {
            var action = this.driven.Actions.Create("tractor-beam:selected");
            this.driven.WriteTargetOffset(action);
            action.Write(this.driven.TractorBeam.Selected);
        }

        private void HandleTractorBeamReleased(object sender, ShipPart arg)
        {
            var action = this.driven.Actions.Create("tractor-beam:released");
            this.driven.WriteTargetOffset(action);
        }

        private void HandleTractorBeamAttached(object sender, FemaleConnectionNode arg)
        {
            var action = this.driven.Actions.Create("tractor-beam:attached");
            action.Write(arg.Parent);
            action.Write(arg.Id);
        }

        private void HandleFiringChanged(object sender, bool arg)
        {
            var action = this.driven.Actions.Create("firing:changed");
            action.Write(this.driven.Firing);
            this.driven.WriteTargetOffset(action);
        }
        #endregion
    }
}
