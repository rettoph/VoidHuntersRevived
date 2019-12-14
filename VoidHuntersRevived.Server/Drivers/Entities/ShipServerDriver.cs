using Guppy;
using Guppy.Attributes;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Utilities;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Entities.ShipParts;
using Guppy.Collections;
using VoidHuntersRevived.Library.Entities.Controllers;
using System.IO;
using Microsoft.Extensions.Logging;

namespace VoidHuntersRevived.Server.Drivers.Entities
{
    /// <summary>
    /// Driver in charge of broadcasting ship specific changes to all
    /// connected clients.
    /// </summary>
    [IsDriver(typeof(Ship))]
    internal sealed class ShipServerDriver : Driver<Ship>
    {
        #region Static Properties
        private static Double TargetPingRate { get; set; } = 75;
        #endregion

        #region Private Fields
        private ActionTimer _targetPingTimer;
        private Vector2 _oldTarget;
        private EntityCollection _entities;
        private ShipBuilder _shipBuilder;
        #endregion

        #region Contructor
        public ShipServerDriver(ShipBuilder shipBuilder, EntityCollection entities, Ship driven) : base(driven)
        {
            _entities = entities;
            _shipBuilder = shipBuilder;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            _targetPingTimer = new ActionTimer(ShipServerDriver.TargetPingRate);

            this.driven.Events.TryAdd<Ship.Direction>("direction:changed", this.HandleDirectionChanged);
            this.driven.Events.TryAdd<Boolean>("firing:changed", this.HandleFiringChanged);
            this.driven.Events.TryAdd<ShipPart>("bridge:changed", this.HandleBridgeChanged);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Check bridge health
            if(this.driven.Bridge != default(ShipPart) && this.driven.Bridge.Health == 0)
            { // If the ship's bridge has no health...
                // Destroy the old bridge...
                this.driven.Bridge.Dispose();

                using (FileStream input = File.OpenRead("Ships/mosquito.vh"))
                    this.driven.SetBridge(_shipBuilder.Import(input));
            }

            _targetPingTimer.Update(
                gameTime:gameTime, 
                action: () =>
                { // on the interval...
                    var action = this.driven.Actions.Create("target:changed", NetDeliveryMethod.UnreliableSequenced, 4);
                    action.Write(_oldTarget = this.driven.Target);
                }, 
                filter: (triggered) => triggered && _oldTarget != this.driven.Target);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// When a server ship's direction is changed,
        /// we must broadcast the update to all connected
        /// clients.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void HandleDirectionChanged(object sender, Ship.Direction direction)
        {
            this.driven.WriteDirection(this.driven.Actions.Create("direction:changed", NetDeliveryMethod.ReliableOrdered, 4), direction);
        }

        /// <summary>
        /// When a server ship's firing value is changed
        /// we need to broadcast the change to all
        /// connected clients.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="value"></param>
        private void HandleFiringChanged(object sender, Boolean value)
        {
            var action = this.driven.Actions.Create("firing:changed", NetDeliveryMethod.ReliableOrdered, 4);
            action.Write(this.driven.Target);
            action.Write(value);
        }

        /// <summary>
        /// When a server's ship's bridge is changed we need
        /// to broadcast the change to all connected clients.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void HandleBridgeChanged(object sender, ShipPart arg)
        {
            var action = this.driven.Actions.Create("bridge:changed", NetDeliveryMethod.ReliableOrdered, 4);
            action.Write(this.driven.Bridge);
        }
        #endregion
    }
}
