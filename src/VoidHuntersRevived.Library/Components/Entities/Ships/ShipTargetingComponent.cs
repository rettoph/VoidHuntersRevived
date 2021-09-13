using Guppy;
using Guppy.Events.Delegates;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Ships;

namespace VoidHuntersRevived.Library.Components.Entities.Ships
{
    public sealed class ShipTargetingComponent : Component<Ship>
    {
        #region Private Fields
        private Vector2 _target;
        #endregion

        #region Public Properties
        /// <summary>
        /// The current ship's world position target.
        /// </summary>
        public Vector2 Target
        {
            get => _target;
            set => this.OnTargetChanged.InvokeIf(_target != value, this.Entity, ref _target, value);
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
