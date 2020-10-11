using Guppy.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Events;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// Partial class primarily designed to manage
    /// the targetting feature of all ship controls.
    /// </summary>
    public partial class Ship
    {
        #region Private Fields
        private Vector2 _target;
        #endregion

        #region Public Properties
        /// <summary>
        /// The calculated world position of the ship's current
        /// target.
        /// </summary>
        public Vector2 Target
        {
            get => _target;
            set
            {
                this.TryInvokeEvent(new ShipEventArgs()
                {
                    Type = ShipEventType.Target,
                    TargetData = value
                });
            }
        }
        #endregion

        #region Lifecycle Methods
        private void Targeting_PreInitialize(ServiceProvider provider)
        {
            this.Events[ShipEventType.Target].ValidateEvent += this.ValidateTargetEvent;
        }

        private void Targeting_Dispose()
        {
            this.Events[ShipEventType.Target].ValidateEvent -= this.ValidateTargetEvent;
        }
        #endregion

        #region Event Handlers
        private bool ValidateTargetEvent(Ship ship, ShipEventArgs args)
            => _target != (_target = args.TargetData);
        #endregion
    }
}
