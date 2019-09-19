using GalacticFighters.Library.Entities.ShipParts;
using Guppy;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Entities
{
    /// <summary>
    /// Simple object used to pick up and interact with
    /// ShipPart objects.
    /// </summary>
    public sealed class TractorBeam : Entity
    {
        #region Private Attributes
        private Guid _selectionId;
        #endregion

        #region Public Attributes
        public ShipPart Selected { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            this.Events.Register<ShipPart>("selected");
            this.Events.Register<ShipPart>("released");
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Check if a given ship part can be selected by the current tractorbeam.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Boolean ValidateTarget(ShipPart target)
        {
            if (target == default(ShipPart))
                return false;
            else if (target.IsBridge)
                return false;
            else if (target.Root.IsBridge && target.Root.BridgeFor?.TractorBeam != this)
                return false;
            else if (target.Reserved.Value)
                return false;
            else if (!target.IsRoot && !target.Root.IsBridge)
                return false;

            return true;
        }

        /// <summary>
        /// Return the selection target, if there is one, from
        /// a given chain component. This will automatically
        /// return the root piece on the chain, the current piece
        /// if you are detaching from the ship, or null if the piece
        /// in invalid.
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public ShipPart FindTarget(ShipPart component)
        {
            if (this.ValidateTarget(component))
                return component;
            else if (this.ValidateTarget(component?.Root))
                return component.Root;
            else
                return default(ShipPart);
        }

        /// <summary>
        /// Attempt to select a recieved shippart targe.
        /// </summary>
        /// <param name="target"></param>
        public Boolean TrySelect(ShipPart target)
        {
            if (target != this.Selected)
            {
                this.TryRelease();

                if ((target = this.FindTarget(target)) != default(ShipPart))
                { // Only attempt anything if the recieved ship part is a valid target
                    _selectionId = target.Reserved.Add();
                    this.Selected = target;

                    this.Events.TryInvoke<ShipPart>(this, "selected", this.Selected);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Attempt to release the current selected ship part, if there is any
        /// </summary>
        public Boolean TryRelease()
        {
            if (this.Selected != default(ShipPart))
            {
                var oldSelected = this.Selected;

                this.Selected.Reserved.Remove(_selectionId);
                this.Selected = null;

                this.Events.TryInvoke<ShipPart>(this, "released", oldSelected);

                return true;
            }

            return false;
        }
        #endregion
    }
}
