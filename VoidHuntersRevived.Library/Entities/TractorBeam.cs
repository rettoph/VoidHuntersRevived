using Guppy;
using Guppy.Configurations;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// The tractor beam entity is a simple object
    /// used to pick up and release free floating ship
    /// parts. It is constantly bound to a ship, and 
    /// must always be a certain "offset" relative to
    /// the ship.
    /// </summary>
    public class TractorBeam : Entity
    {
        #region Private Fields
        /// <summary>
        /// The distance a target may be from the ship and still get selected.
        /// </summary>
        private Single _reach;
        private Guid _targetFocus;
        #endregion

        #region Public Attributes
        public readonly Ship Ship;
        public Boolean Selecting { get { return this.Selected != null; } }
        public ShipPart Selected { get; private set; }
        public Vector2 Offset { get; private set; }
        public Vector2 Position { get { return this.Ship.Bridge.Position + this.Offset; } }
        #endregion

        #region Events
        public event EventHandler<ShipPart> OnSelect;
        public event EventHandler<ShipPart> OnRelease;
        #endregion

        #region Constructors
        public TractorBeam(Ship ship, EntityConfiguration configuration, IServiceProvider provider) : base(configuration, provider)
        {
            _reach = 20;
            this.Ship = ship;
        }
        #endregion

        #region Frame Methods
        protected override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if(this.Selecting)
            { // If something is selected, update its position
                this.Selected.SetTransform(
                    this.Position, 
                    this.Selected.Rotation);
            }
        }
        #endregion

        #region Interaction Methods
        /// <summary>
        /// Attempt to select a given target.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Boolean TrySelect(ShipPart target)
        {
            // Ensure that the proper target is recieved
            target = this.GetTarget(target);

            if (!this.ValidateTarget(target))
                return false;

            // Attempt to release the old target (if there is any)
            this.TryRelease();

            // Focus the new target
            this.Selected = target;
            this.Selected.SleepingAllowed = false;
            _targetFocus = this.Selected.Focused.Add();

            this.OnSelect?.Invoke(this, this.Selected);
            return true;
        }

        public Boolean TryRelease()
        {
            if (this.Selected == null)
                return false;

            var oldTarget = this.Selected;

            this.Selected.Focused.Remove(_targetFocus);
            this.Selected.SleepingAllowed = true;
            this.Selected = null;

            this.OnRelease?.Invoke(this, oldTarget);
            return true;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Check if a specified ship part can be selected by the
        /// current tractor beam.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Boolean ValidateTarget(ShipPart target)
        {
            // If there is no bridge bound to the current tractor beam...
            if (this.Ship.Bridge == null)
                return false;

            // If the target is null...
            if (target == null)
                return false;

            // If the ship part is focused (usually means selected by another tractor beam)
            if (target.Focused.Value)
                return false;

            // If the part is too far away from the current ship...
            if (Vector2.Distance(target.Position, this.Ship.Bridge.Position) > _reach)
                return false;

            // If the target is attached to another ship...
            if (target.Root.IsBridge && target.Root.BridgeFor != this.Ship)
                return false;

            // If the target is the current ship bridge...
            if (this.Ship.Bridge == target)
                return false;

            return true;
        }

        /// <summary>
        /// If the input target is part of a chain, return the root most item
        /// That is, unless the target is part of a ship then return the target
        /// directly. If the target is not valid at all then return null.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public ShipPart GetTarget(ShipPart target)
        {
            if (!this.ValidateTarget(target))
                return default(ShipPart);
            if (target.Root.IsBridge)
                return target;

            return target.Root;
        }
        #endregion

        #region Position Methods
        public void SetOffset(Vector2 offset)
        {
            this.Offset = offset;
        }
        #endregion
    }
}
