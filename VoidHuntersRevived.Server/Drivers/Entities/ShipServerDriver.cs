using Guppy;
using Guppy.Attributes;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Server.Drivers.Entities
{
    /// <summary>
    /// Driver in charge of broadcasting ship specific changes to all
    /// connected clients.
    /// </summary>
    [IsDriver(typeof(Ship))]
    internal sealed class ShipServerDriver : Driver<Ship>
    {
        #region Contructor
        public ShipServerDriver(Ship driven) : base(driven)
        {
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.driven.Events.TryAdd<Ship.Direction>("direction:changed", this.HandleDirectionChanged);
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
            this.driven.WriteDirection(this.driven.Actions.Create("direction:changed", NetDeliveryMethod.UnreliableSequenced, 4), direction);
        }
        #endregion
    }
}
