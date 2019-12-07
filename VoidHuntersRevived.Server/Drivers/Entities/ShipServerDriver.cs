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

namespace VoidHuntersRevived.Server.Drivers.Entities
{
    /// <summary>
    /// Driver in charge of broadcasting ship specific changes to all
    /// connected clients.
    /// </summary>
    [IsDriver(typeof(Ship))]
    internal sealed class ShipServerDriver : Driver<Ship>
    {
        #region Private Fields
        private ActionTimer _targetPingTimer;
        private Vector2 _oldTarget;
        #endregion

        #region Contructor
        public ShipServerDriver(Ship driven) : base(driven)
        {
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            _targetPingTimer = new ActionTimer(100);

            this.driven.Events.TryAdd<Ship.Direction>("direction:changed", this.HandleDirectionChanged);
            this.driven.Events.TryAdd<Boolean>("firing:changed", this.HandleFiringChanged);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _targetPingTimer.Update(
                gameTime:gameTime, 
                action: () =>
                { // on the interval...
                    var action = this.driven.Actions.Create("target:changed", NetDeliveryMethod.UnreliableSequenced, 4);
                    action.Write(_oldTarget = this.driven.Target);
                }, 
                filter: () => 
                { // Only boradcast a message if the target has changed...
                    return _oldTarget != this.driven.Target;
                });
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
        #endregion
    }
}
