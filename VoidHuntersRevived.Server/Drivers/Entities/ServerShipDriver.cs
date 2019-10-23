﻿using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using Guppy;
using Guppy.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Entities.ShipParts.ConnectionNodes;
using Microsoft.Xna.Framework;
using Guppy.Collections;
using System.IO;
using VoidHuntersRevived.Library.Utilities;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using VoidHuntersRevived.Library.Extensions;

namespace VoidHuntersRevived.Server.Drivers.Entities
{
    [IsDriver(typeof(Ship), 200)]
    public class ServerShipDriver : Driver<Ship>
    {
        #region Static Fields
        public static Double UpdateTargetRate { get; set; } = 250;
        #endregion

        #region Private Fields
        private Vector2 _oldTarget;
        private ShipBuilder _builder;
        private Interval _interval;
        private EntityCollection _entities;
        #endregion

        #region Constructor
        public ServerShipDriver(EntityCollection entities, Interval interval, ShipBuilder builder, Ship driven) : base(driven)
        {
            _interval = interval;
            _builder = builder;
            _entities = entities;
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

            if(_interval.Is(ServerShipDriver.UpdateTargetRate) && Vector2.Distance(_oldTarget,this.driven.TargetOffset) > 0.25f)
            {
                var action = this.driven.Actions.Create("target:changed", NetDeliveryMethod.Unreliable, 5);
                action.Write(this.driven.TargetOffset);

                _oldTarget = this.driven.TargetOffset;
            }

            if(this.driven.Bridge != null && this.driven.Bridge.Health <= 0)
            { // When the bridge is low health, copy the ship over to an explosion
                _entities.Create<Explosion>("explosion", e =>
                {
                    e.SetSource(this.driven.Bridge);
                    this.driven.SetBridge(null);
                });
            }
        }
        #endregion

        #region Event Handlers
        private void HandleBridgeChanged(object sender, ShipPart bridge)
        { 
            this.driven.WriteBridge(this.driven.Actions.Create("bridge:changed", NetDeliveryMethod.ReliableOrdered, 3));
        }

        private void HandleDirectionChanged(object sender, Ship.Direction direction)
        {
            this.driven.WriteDirection(this.driven.Actions.Create("direction:changed", NetDeliveryMethod.UnreliableSequenced, 3), direction);
        }

        private void HandleTractorBeamSelected(object sender, ShipPart arg)
        {
            var action = this.driven.Actions.Create("tractor-beam:selected", NetDeliveryMethod.ReliableOrdered, 3);
            this.driven.WriteTargetOffset(action);
            action.Write(this.driven.TractorBeam.Selected);
        }

        private void HandleTractorBeamReleased(object sender, ShipPart arg)
        {
            var action = this.driven.Actions.Create("tractor-beam:released", NetDeliveryMethod.ReliableOrdered, 3);
            this.driven.WriteTargetOffset(action);
        }

        private void HandleTractorBeamAttached(object sender, FemaleConnectionNode arg)
        {
            var action = this.driven.Actions.Create("tractor-beam:attached", NetDeliveryMethod.ReliableOrdered, 3);
            action.Write(arg.Parent);
            action.Write(arg.Id);
        }

        private void HandleFiringChanged(object sender, bool arg)
        {
            var action = this.driven.Actions.Create("firing:changed", NetDeliveryMethod.ReliableOrdered, 3);
            action.Write(this.driven.Firing);
            this.driven.WriteTargetOffset(action);
        }
        #endregion
    }
}
