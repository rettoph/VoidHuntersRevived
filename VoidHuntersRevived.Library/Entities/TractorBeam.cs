using GalacticFighters.Library.Entities.ShipParts;
using Guppy;
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

        #region Utility Methods
        /// <summary>
        /// Check if a given ship part can be selected by the current tractorbeam.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Boolean ValidateTarget(ShipPart target)
        {
            if (target.IsBridge)
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
            else if (this.ValidateTarget(component.Root))
                return component.Root;
            else
                return default(ShipPart);
        }

        /// <summary>
        /// Attempt to select a recieved shippart targe.
        /// </summary>
        /// <param name="target"></param>
        public void TrySelect(ShipPart target)
        {
            if ((target = this.FindTarget(target)) != default(ShipPart))
            { // Only attempt anything if the recieved ship part is a valid target
                _selectionId = target.Reserved.Add();
                this.Selected = target;
            }
        }
        #endregion
    }
}
