using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.CustomEventArgs;
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
                var thrust = new Vector2(0.25f, 0);

                if (_directions[Direction.Forward])
                    this.Bridge.ApplyLinearImpulse(Vector2.Transform(thrust, Matrix.CreateRotationZ(this.Bridge.Rotation)));
                if (_directions[Direction.Backward])
                    this.Bridge.ApplyLinearImpulse(Vector2.Transform(thrust, Matrix.CreateRotationZ(this.Bridge.Rotation + MathHelper.Pi)));

                if (_directions[Direction.TurnLeft])
                    this.Bridge.ApplyAngularImpulse(-0.03f);
                if (_directions[Direction.TurnRight])
                    this.Bridge.ApplyAngularImpulse(0.03f);
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
        #endregion
    }
}
