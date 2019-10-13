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
using Guppy.Collections;
using System.IO;
using GalacticFighters.Library.Utilities;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using GalacticFighters.Library.Extensions;

namespace GalacticFighters.Server.Drivers.Entities
{
    [IsDriver(typeof(Ship))]
    public class ServerShipDriver : Driver<Ship>
    {
        #region Static Fields
        public static Double UpdateTargetRate { get; set; } = 120;
        #endregion

        #region Private Fields
        private Vector2 _oldTarget;
        private ShipBuilder _builder;
        private Interval _interval;
        private Int32 _lives = 0;
        #endregion

        #region Constructor
        public ServerShipDriver(Interval interval, ShipBuilder builder, Ship driven) : base(driven)
        {
            _interval = interval;
            _builder = builder;
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

            if(_interval.Is(ServerShipDriver.UpdateTargetRate) && _oldTarget != this.driven.TargetOffset)
            {
                var action = this.driven.Actions.Create("target:changed", NetDeliveryMethod.Unreliable, 2);
                action.Write(this.driven.TargetOffset);

                _oldTarget = this.driven.TargetOffset;
            }

            if(this.driven.Bridge != null && this.driven.Bridge.Health <= 0)
            { // When the bridge is low health, blow up the ship
                this.driven.Bridge.Dispose();

                if (_lives < 15)
                {
                    var ships = new String[] { "mosquito", "turret-01", "turret-02" };
                    var rand = new Random();
                    using (FileStream import = File.OpenRead($"Ships/{ships[rand.Next(0, 1)]}.vh"))
                        this.driven.SetBridge(_builder.Import(import));


                    this.driven.Bridge.SetPosition(new Vector2(rand.NextSingle(-100, 100), rand.NextSingle(-100, 100)), rand.NextSingle(-3, 3));

                    _lives++;
                }
            }
        }
        #endregion

        #region Event Handlers
        private void HandleBridgeChanged(object sender, ShipPart bridge)
        { 
            this.driven.WriteBridge(this.driven.Actions.Create("bridge:changed", NetDeliveryMethod.ReliableOrdered));
        }

        private void HandleDirectionChanged(object sender, Ship.Direction direction)
        {
            this.driven.WriteDirection(this.driven.Actions.Create("direction:changed", NetDeliveryMethod.ReliableOrdered), direction);
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
