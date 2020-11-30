using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// Partial ship class designed to contain directional
    /// movement & thruster specific code.
    /// </summary>
    public partial class Ship
    {
        #region Enums
        /// <summary>
        /// A helper flags enum used
        /// to store directional data
        /// about the current ship.
        /// </summary>
        [Flags]
        public enum Direction
        {
            None = 0,
            Forward = 1,
            Right = 2,
            Backward = 4,
            Left = 8,
            TurnLeft = 16,
            TurnRight = 32
        }
        #endregion

        #region Structs
        /// <summary>
        /// Helper struct to contain a direction state. 
        /// This allows the passing of a direction changed 
        /// event's data through a single property.
        /// </summary>
        public struct DirectionState
        {
            /// <summary>
            /// The direction flag whose <see cref="State"/> is defined.
            /// </summary>
            public readonly Direction Direction;

            /// <summary>
            /// The state of the current <see cref="Direction"/>.
            /// </summary>
            public readonly Boolean State;

            #region Constructor
            /// <summary>
            /// Create a new DirectionState instance.
            /// </summary>
            /// <param name="direction">The direction flag whose <see cref="state"/> is defined.</param>
            /// <param name="state">The state of the recieved <see cref="direction"/>.</param>
            public DirectionState(Direction direction, bool state)
            {
                this.Direction = direction;
                this.State = state;
            }
            #endregion
        }
        #endregion

        #region Public Attributes
        /// <summary>
        /// The current Ship's active directions. This can be updated via 
        /// the <see cref="TrySetDirection(Direction, bool)"/> helper method.
        /// </summary>
        public Ship.Direction ActiveDirections { get; private set; }
        #endregion

        #region Events
        /// <summary>
        /// Event invoked when the <see cref="ActiveDirections"/> property value
        /// is updated via the <see cref="TrySetDirection(DirectionState)"/> method.
        /// 
        /// This will contain the changed direction and its new state.
        /// </summary>
        public event OnEventDelegate<Ship, DirectionState> OnDirectionChanged;
        #endregion

        #region Helper Methods
        /// <summary>
        /// Attempt to set a directional flag within the current ship.
        /// If the change goes through, the <see cref="OnDirectionChanged"/>
        /// event will be invoked.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Boolean TrySetDirection(Ship.Direction direction, Boolean state)
            => this.TrySetDirection(new DirectionState(direction, state));

        /// <summary>
        /// Attempt to set a directional flag within the current ship.
        /// If the change goes through, the <see cref="OnDirectionChanged"/>
        /// event will be invoked.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Boolean TrySetDirection(DirectionState args)
        {
            if (args.State && (this.ActiveDirections & args.Direction) == 0)
                this.ActiveDirections |= args.Direction;
            else if (!args.State && (this.ActiveDirections & args.Direction) != 0)
                this.ActiveDirections &= ~args.Direction;
            else
                return false;

            // If we've made it this fat then we know that the directional change was a success.
            // Invoke the direction changed event now.
            this.OnDirectionChanged?.Invoke(this, args);

            return true;
        }
        #endregion
    }
}
