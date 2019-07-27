using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.CustomEventArgs;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// Specific file implimenting movement specific ship
    /// functionality.
    /// </summary>
    public partial class Ship
    {
        #region Private Fields
        private Dictionary<Direction, Boolean> _directions;
        private List<Thruster> _thrusters;
        private Dictionary<Direction, List<Thruster>> _thrusterDirections;
        #endregion

        #region Events
        public event EventHandler<DirectionChangedEventArgs> OnDirectionChanged;
        #endregion

        #region Initialization Methods
        /// <summary>
        /// Automatically called within this.Initialize.
        /// This is found inside Ship.cs
        /// </summary>
        protected virtual void InitializeMovement()
        {
            _directions = (Enum.GetValues(typeof(Direction)) as Direction[]).ToDictionary(d => d, d => false);

            _thrusters = new List<Thruster>();
            _thrusterDirections = (Enum.GetValues(typeof(Direction)) as Direction[])
                .ToDictionary(
                    keySelector: d => d,
                    elementSelector: d => new List<Thruster>());

        }
        #endregion

        #region Frame Methods
        /// <summary>
        /// Run movement specific update logic. Called by this.update() in
        /// Ship.cs
        /// </summary>
        /// <param name="gameTime"></param>
        protected virtual void UpdateMovement(GameTime gameTime)
        {
            if (this.Bridge != null)
            { // We only need to bother moving the ship if there is a bridge defined...
                this.Bridge.AngularDamping = MathHelper.Lerp(this.Bridge.AngularDamping, (_directions[Direction.TurnLeft] || _directions[Direction.TurnRight]) ? 1f : 1.5f, 0.25f);

                foreach (Thruster thruster in _thrusters)
                    thruster.SetActive(false);

                foreach (KeyValuePair<Direction, Boolean> kvp in _directions)
                    if (kvp.Value)
                        foreach (Thruster thruster in _thrusterDirections[kvp.Key])
                            thruster.SetActive(true);
                // var thrust = new Vector2(0.25f, 0);
                // 
                // if (_directions[Direction.Forward])
                //     this.Bridge.ApplyLinearImpulse(Vector2.Transform(thrust, Matrix.CreateRotationZ(this.Bridge.Rotation)));
                // if (_directions[Direction.Backward])
                //     this.Bridge.ApplyLinearImpulse(Vector2.Transform(thrust, Matrix.CreateRotationZ(this.Bridge.Rotation + MathHelper.Pi)));
                // 
                // if (_directions[Direction.TurnLeft])
                //     this.Bridge.ApplyAngularImpulse(-0.03f);
                // if (_directions[Direction.TurnRight])
                //     this.Bridge.ApplyAngularImpulse(0.03f);
            }

        }
        #endregion

        #region Utlity Methods
        public Boolean GetDirection(Direction direction)
        {
            return _directions[direction];
        }

        public void SetDirection(Direction direction, Boolean value)
        {
            if (_directions[direction] != value)
            {
                this.logger.LogDebug($"Setting Ship({this.Id}) Direction<{direction}> to {value}...");

                _directions[direction] = value;

                this.OnDirectionChanged?.Invoke(this, new DirectionChangedEventArgs(direction, !value, value));
            }
        }

        /// <summary>
        /// This is automatically called when the bridge connection nodes
        /// are remapped, and is used to map the current ships thrusters.
        /// </summary>
        private void RemapThrusters()
        {
            _thrusters.Clear();
            _thrusters.AddRange(this.children
                .Where(c => typeof(Thruster).IsAssignableFrom(c.GetType()))
                .Select(c => c as Thruster));

            // Clear the old thruster mapping...
            foreach (Direction direction in Enum.GetValues(typeof(Direction)) as Direction[])
                _thrusterDirections[direction].Clear();

            // Remap the thruster data
            var directions = new List<Direction>();
            foreach (Thruster thruster in _thrusters)
                foreach (Direction direction in thruster.GetDirections(ref directions))
                    _thrusterDirections[direction].Add(thruster);
        }
        #endregion
    }
}
