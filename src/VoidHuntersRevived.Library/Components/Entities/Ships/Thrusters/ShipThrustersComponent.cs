using Guppy;
using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Extensions.System.Collections;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Globals.Constants;
using VoidHuntersRevived.Library.Structs;

namespace VoidHuntersRevived.Library.Components.Entities.Ships
{
    public abstract class ShipThrustersComponent : ShipShipPartsComponent<Thruster>
    {
        #region Public Attributes
        /// <summary>
        /// The current Ship's active directions. This can be updated via 
        /// the <see cref="TrySetDirection(Direction, bool)"/> helper method.
        /// </summary>
        public Direction ActiveDirections { get; private set; }
        #endregion

        #region Events
        /// <summary>
        /// Event invoked when the <see cref="ActiveDirections"/> property value
        /// is updated via the <see cref="TrySetDirection(DirectionState)"/> method.
        /// 
        /// This will contain the changed direction and its new state.
        /// </summary>
        public event OnEventDelegate<ShipThrustersComponent, DirectionState> OnDirectionChanged;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.Entity.OnUpdate += this.Update;
        }

        protected override void PreInitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.PreInitializeRemote(provider, networkAuthorization);

            this.Entity.Messages.Add(Messages.Ship.DirectionChanged, Guppy.Network.Constants.MessageContexts.InternalUnreliableDefault);
        }

        protected override void PostRelease()
        {
            base.PostRelease();

            this.Entity.OnUpdate -= this.Update;
        }
        #endregion

        #region Frame Methods
        private void Update(GameTime gameTime)
        {
            if (this.ActiveDirections == Direction.None || this.Entity.Chain == default)
                return;

            Vector2 impulse = Vector2.Zero;

            if((this.ActiveDirections & Direction.Forward) != 0)
            {
                impulse -= Vector2.UnitY * 10;
            }
            if ((this.ActiveDirections & Direction.TurnRight) != 0)
            {
                impulse += Vector2.UnitX * 10;
            }
            if ((this.ActiveDirections & Direction.Backward) != 0)
            {
                impulse += Vector2.UnitY * 10;
            }
            if ((this.ActiveDirections & Direction.TurnLeft) != 0)
            {
                impulse -= Vector2.UnitX * 10;
            }

            this.Entity.Chain.Body.ApplyLinearImpulse(impulse * (Single)gameTime.ElapsedGameTime.TotalSeconds);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Attempt to set a directional flag within the current ship.
        /// If the change goes through, the <see cref="OnDirectionChanged"/>
        /// event will be invoked.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Boolean TrySetDirection(Direction direction, Boolean value)
            => this.TrySetDirection(new DirectionState(direction, value));

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
    }
}
