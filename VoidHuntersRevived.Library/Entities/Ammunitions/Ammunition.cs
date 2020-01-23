using Guppy;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Configurations;
using VoidHuntersRevived.Library.Entities.ShipParts.Weapons;

namespace VoidHuntersRevived.Library.Entities.Ammunitions
{
    /// <summary>
    /// Base class representing a generic type
    /// of ammunition.
    /// 
    /// Projectile & Laser should both inherit
    /// from this.
    /// </summary>
    public abstract class Ammunition : Entity
    {
        #region Public Attributes
        public Weapon Weapon { get; internal set; }
        public Vector2 Position { get; protected internal set; }
        public Single Rotation { get; protected internal set; }

        /// <summary>
        /// The maximum allowed age before the ammunition
        /// self disposes.
        /// </summary>
        public Double MaxAge { get; set; }

        /// <summary>
        /// The current age of the ammunition.
        /// </summary>
        public Double Age { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.Age = 0;
            this.MaxAge = this.MaxAge;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Age += gameTime.ElapsedGameTime.TotalMilliseconds;

            // Auto dispose of the projectile when it hits the max allowed age.
            if (this.MaxAge != -1 && this.Age >= this.MaxAge)
                this.Dispose();
        }
        #endregion
    }
}
