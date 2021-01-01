using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Extensions.System.Collections;
using Guppy.Extensions.Microsoft.Xna.Framework;
using Guppy.Extensions.System;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Library.Extensions.Microsoft.Xna;
using VoidHuntersRevived.Library.Extensions.System.Collections;
using VoidHuntersRevived.Library.Utilities;

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
        public enum Direction : Byte
        {
            None = 0,
            Forward = 1,
            Right = 2,
            Backward = 4,
            Left = 8,
            TurnRight = 16,
            TurnLeft = 32
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

        #region Private Fields
        /// <summary>
        /// A dictionary containing each thruster proken by the
        /// directions they will move the ship. Note, single
        /// thruster instances can appear multiple times as they can
        /// move the ship in several directions.
        /// 
        /// This list will be maintained based on the <see cref="OnClean"/>
        /// event.
        /// </summary>
        private Dictionary<Ship.Direction, IList<Thruster>> _directionThrusters;

        /// <summary>
        /// A list property containing all thrusters currently within
        /// the ship.
        /// 
        /// This list will be maintained based on the <see cref="OnClean"/>
        /// event.
        /// </summary>
        private List<Thruster> _thrusters;
        #endregion

        #region Public Attributes
        /// <summary>
        /// The current Ship's active directions. This can be updated via 
        /// the <see cref="TrySetDirection(Direction, bool)"/> helper method.
        /// </summary>
        public Ship.Direction ActiveDirections { get; private set; }

        /// <summary>
        /// A dictionary containing each thruster proken by the
        /// directions they will move the ship, referencing <seealso cref="_directionThrusters"/>. Note, single
        /// thruster instances can appear multiple times as they can
        /// move the ship in several directions.
        /// 
        /// This list will be maintained based on the <see cref="OnClean"/>
        /// event.
        /// </summary>
        public IReadOnlyDictionary<Ship.Direction, IList<Thruster>> DirectionThrusters => _directionThrusters;

        /// <summary>
        /// A list property containing all thrusters currently within
        /// the ship, referencing <seealso cref="_thrusters"/>.
        /// 
        /// This list will be maintained based on the <see cref="OnClean"/>
        /// event.
        /// </summary>
        public IReadOnlyList<Thruster> Thrusters => _thrusters;
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

        #region Lifecycle Methods
        private void Thrusters_Create(ServiceProvider provider)
        {
            _thrusters = new List<Thruster>();
            _directionThrusters = DictionaryHelper.BuildEnumDictionary<Ship.Direction, IList<Thruster>>(d => new List<Thruster>());

            this.OnUpdate += this.Thrusters_Update;
            this.OnClean += Ship.Thrusters_HandleClean;
            this.OnDirectionChanged += Ship.Thrusters_HandleDirectionChanged;
        }

        private void Thrusters_PreInitialize(ServiceProvider provider)
        {
            // reset the ships current directions if needed...
            this.ActiveDirections = Direction.None;
        }

        private void Thrusters_Dispose()
        {
            _thrusters.Clear();
            _directionThrusters.Clear();

            this.OnUpdate -= this.Thrusters_Update;
            this.OnClean -= Ship.Thrusters_HandleClean;
        }
        #endregion

        #region Frame Methods
        /// <summary>
        /// Update method to handle Thruster specific functionality.
        /// </summary>
        /// <param name="gameTime"></param>
        private void Thrusters_Update(GameTime gameTime)
        {
            // Update all internal thrusters...
            _thrusters.ForEach(t => t.TryUpdate(gameTime));
        }
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
            if ((args.Direction & (args.Direction - 1)) != 0)
                throw new Exception("Unable to set multiple directions at once.");

            if (args.State && (this.ActiveDirections & args.Direction) == 0)
                this.ActiveDirections |= args.Direction;
            else if (!args.State && (this.ActiveDirections & args.Direction) != 0)
                this.ActiveDirections &= ~args.Direction;
            else
                return false;

            // If we've made it this far then we know that the directional change was a success.
            // Invoke the direction changed event now.
            this.OnDirectionChanged?.Invoke(this, args);

            return true;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// When the ship is cleaned we should iterate through
        /// each child thruster and catalogue the direction it moves
        /// the ship in.
        /// </summary>
        /// <param name="sender"></param>
        private static void Thrusters_HandleClean(Ship sender)
        {
            lock (sender._thrusters)
            {
                // Clear then repopulate the internal thrusters list...
                sender._thrusters.Clear();

                // Clear all old stored direcitonal data...
                foreach (IList<Thruster> thrusters in sender._directionThrusters.Values)
                    thrusters.Clear();

                if (sender.Bridge != default)
                { // If there is abridge, search for all thrusters within the chain.
                    sender._thrusters.AddRange(sender.Bridge?.Items(c => c is Thruster).Select(c => c as Thruster) ?? Enumerable.Empty<Thruster>());

                    // Rebuild the directionThrusters dictionary...
                    foreach (Thruster thruster in sender.Thrusters)
                    { // Iterate through all internal thrusters...
                        // Reset the thruster's ActiveDirections...
                        thruster.ActiveDirections = Direction.None;

                        foreach(Ship.Direction direction in thruster.GetDirections()) // Iterate through all directions the current thruster activates...
                            sender._directionThrusters[direction].Add(thruster);
                    }


                    // re-enable currently active thrusters...
                    foreach (Ship.Direction active in sender.ActiveDirections.GetFlags())
                        foreach (Thruster thruster in sender.DirectionThrusters[active])
                            thruster.ActiveDirections |= active;
                }
            }
        }

        /// <summary>
        /// When the direction changes we should update the
        /// <see cref="Thruster.ActiveDirections"/> flags
        /// based on the <see cref="Ship.DirectionThrusters"/>
        /// state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void Thrusters_HandleDirectionChanged(Ship sender, DirectionState args)
        {
            if (args.State)
                foreach (Thruster thruster in sender.DirectionThrusters[args.Direction])
                    thruster.ActiveDirections |= args.Direction;
            else
                foreach (Thruster thruster in sender.DirectionThrusters[args.Direction])
                    thruster.ActiveDirections &= ~args.Direction;
        }
        #endregion
    }
}
