using GalacticFighters.Library.Entities.ShipParts.Weapons;
using Guppy;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Entities.Ammunitions
{
    /// <summary>
    /// Ammunition represents the base 
    /// </summary>
    public class Ammunition : Entity
    {
        #region Public Attributes
        /// <summary>
        /// The weapon that launched the current ammunition.
        /// </summary>
        public Weapon Weapon { get; internal set; }

        /// <summary>
        /// The current position of the given ammunition.
        /// </summary>
        public Vector2 Position { get; internal set; }

        public Single Rotation { get; internal set; }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            // Set the default position of the ammunition to the firing weapons body anchor
            this.Position = this.Weapon.WorldBodyAnchor;

            this.Rotation = this.Weapon.Rotation + this.Weapon.JointAngle;
        }
        #endregion
    }
}
