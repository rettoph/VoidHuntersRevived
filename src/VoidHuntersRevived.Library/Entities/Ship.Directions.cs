using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Events;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// Partial ship class designed to contain directional
    /// movement specific code.
    /// </summary>
    public partial class Ship
    {
        #region Public Attributes
        public Ship.Direction ActiveDirections { get; private set; }
        #endregion

        #region Lifecycle Methods
        private void Directions_PreInitialize(ServiceProvider provider)
        {
            this.Events[ShipEventType.Direction].ValidateEvent += this.ValidateDirectionEvent;
        }

        private void Directions_Dispose()
        {
            this.Events[ShipEventType.Direction].ValidateEvent -= this.ValidateDirectionEvent;
        }
        #endregion

        #region Helper Methods
        public Boolean TrySetDirection(Ship.Direction direction, Boolean state)
            => this.TryInvokeEvent(new ShipEventArgs()
            {
                Type = ShipEventType.Direction,
                DirectionData = new ShipDirectionData()
                {
                    Direction = direction,
                    State = state
                }
            });
        #endregion

        #region Event Handlers
        /// <summary>
        /// Set a specified directional flag.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="value"></param>
        private Boolean ValidateDirectionEvent(Ship ship, ShipEventArgs args)
        {
            if (args.DirectionData.State && !this.ActiveDirections.HasFlag(args.DirectionData.Direction))
                this.ActiveDirections |= args.DirectionData.Direction;
            else if (!args.DirectionData.State && this.ActiveDirections.HasFlag(args.DirectionData.Direction))
                this.ActiveDirections &= ~args.DirectionData.Direction;
            else
                return false;

            return true;
        }
        #endregion
    }
}
