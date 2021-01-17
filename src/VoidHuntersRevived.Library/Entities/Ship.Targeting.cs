using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// Partial class primarily designed to manage
    /// the targetting feature of all ship controls.
    /// </summary>
    public partial class Ship
    {
        #region Private Fields
        /// <summary>
        /// The private field referencing the current 
        /// ship's world position target.
        /// </summary>
        private Vector2 _target;
        #endregion

        #region Public Properties
        /// <summary>
        /// The current ship's world position target.
        /// </summary>
        public Vector2 Target
        {
            get => _target;
            set => this.OnTargetChanged.InvokeIf(_target != value, this, ref _target, value);
        }
        #endregion

        #region Events
        /// <summary>
        /// A simple event invoked when the <see cref="Target"/> property
        /// is updated.
        /// </summary>
        public event OnEventDelegate<Ship, Vector2> OnTargetChanged;
        #endregion
    }
}
